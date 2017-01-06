using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using Utilities;
using vk.Models;
using vk.Models.Files;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;
using vk.Views;

namespace vk.ViewModels {
   public class UploadViewModel : BindableBase, IViewModel, IDropTarget {
      private string _text;
      private string _filepath;
      private string _dateString;
      private int _dateUnix;
      private float _uploadProgress;
      private bool _isBusy;
      public ICommand UploadCommand { get; set; }
      public ICommand OpenFilesCommand { get; set; }

      public SmartCollection<string> Files { get; set; }

      public string Text {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }

      public string filepath {
         get { return _filepath; }
         set { SetProperty(ref _filepath, value); }
      }

      public string DateString {
         get { return _dateString; }
         set { SetProperty(ref _dateString, value); }
      }

      public int DateUnix {
         get { return _dateUnix; }
         set {
            SetProperty(ref _dateUnix, value);
            DateString = UnixTimeConverter.ToDateTime(_dateUnix).ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture);
         }
      }

      public WallControl Wall { get; private set; }

      public ICommand BrowseCommand { get; set; }

      public float UploadProgress {
         get { return _uploadProgress; }
         set { SetProperty(ref _uploadProgress, value); }
      }

      public Progress<int> Progress { get; set; }

      public void PrepareImages(UploadInfo info) {
         Files.Clear();
         Files.AddRange(info.Files);

         Wall.WallHolder = info.Wall.WallHolder;
         Wall.PullWithScheduleHightlight(new MissingPostFilter(), new Schedule());

         if (info.DateOverride == -1) {
            var firstMissed = Wall.Items.FirstOrDefault();
            if (firstMissed == null) return;
            DateUnix = firstMissed.Post.DateUnix;
         }
         else {
            DateUnix = info.DateOverride;
         }
      }

      public void ImportFiles(IEnumerable<string> files) {
         filepath = files.First();
         //var filter = new FileFilter(new ImageExtensionChecker());
         //Files.AddRange(filter.FilterOut(files));
      }

      private Dispatcher dispatcher;

      public UploadViewModel() {
         dispatcher = Dispatcher.CurrentDispatcher;
         Files = new SmartCollection<string>();
         Wall = App.Container.Resolve<WallControl>();

         UploadCommand = new DelegateCommand(uploadCommandExecute);
         OpenFilesCommand = new DelegateCommand(openFilesCommandExecute);
         BrowseCommand = new DelegateCommand(browseCommandExecute);

         Progress = new Progress<int>(percent => {
            UploadProgress = percent;
         });
      }


      private void browseCommandExecute() {
         var openFile = new OpenFileDialog();
         var checker = App.Container.Resolve<ImageExtensionChecker>();

         openFile.Filter = checker.GetFileFilter();
         var result = openFile.ShowDialog();
         if (result == true) {
            var file = openFile.FileName;
            if (checker.IsFileHaveValidExtension(file)) {
               filepath = file;
            }
         }
      }

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }

      private async void uploadCommandExecute() {
         if (IsBusy) return;

         IsBusy = true;
         try {
            var getUploadServerMethod = App.Container.Resolve<PhotosGetWallUploadSever>();
            var uploadServer = getUploadServerMethod.Get(-Wall.WallHolder.ID);

            if (!string.IsNullOrEmpty(filepath) || File.Exists(filepath)) {

               await Task.Run(() => {
                  using (var wc = new WebClient()) {
                     wc.UploadFileCompleted += onUploadFileCompleted;
                     wc.UploadProgressChanged += onUploadProgressChanged;
                     wc.UploadFileAsync(new Uri(uploadServer.UploadUrl), "POST", filepath);
                  }
               });
            }
         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message);
            IsBusy = false;
         }
      }

      private void onUploadProgressChanged(object sender, UploadProgressChangedEventArgs e) {
         UploadProgress = e.ProgressPercentage;
      }

      private void onUploadFileCompleted(object sender, UploadFileCompletedEventArgs e) {
         var uploadResponse = Encoding.UTF8.GetString(e.Result);

         var attachments = new List<string>();
         var wallPostMethod = App.Container.Resolve<WallPost>();

         if (!string.IsNullOrEmpty(uploadResponse)) {
            var savePhotoMethod = App.Container.Resolve<PhotosSaveWallPhoto>();
            var savePhotoProperty = savePhotoMethod.Save(-Wall.WallHolder.ID, uploadResponse);

            var result = savePhotoProperty;
            if (result != null) {
               var userId = App.Container.Resolve<AccessToken>().UserID;
               attachments.AddRange(result.Response.Select(photoResponse => $"photo{userId}_{photoResponse.Id}"));
            }
         }

         wallPostMethod.Post(-Wall.WallHolder.ID, Text, false, true, DateUnix, attachments);

         dispatcher.Invoke(() => {
            Text = "";
            filepath = "";

            Messenger.Broadcast("refresh");
            getNextDate();

            IsBusy = false;
         });
      }

      private void getNextDate() {
         Wall.PullWithScheduleHightlight(new MissingPostFilter(), new Schedule());
         var firstMissed = Wall.Items.FirstOrDefault((p) => p.Post.DateUnix > DateUnix);
         if (firstMissed == null) {
            firstMissed = Wall.Items.FirstOrDefault();
            if (firstMissed == null) return;
            DateUnix = firstMissed.Post.DateUnix;
            return;
         }
         DateUnix = firstMissed.Post.DateUnix;
      }

      private void openFilesCommandExecute() {
         var extChecker = App.Container.Resolve<ImageExtensionChecker>();

         var dialog = new OpenFileDialog {
            Filter = extChecker.GetFileFilter(), Multiselect = true,
         };

         var result = dialog.ShowDialog();

         if (result == true) {
            if (dialog.CheckPathExists) {
               ImportFiles(dialog.FileNames);
            }
         }
      }

      public void OnLoad() {
      }

      public void OnClosing() {
      }

      public void OnClosed() {
      }

      public void DragOver(IDropInfo dropInfo) {

      }

      public void Drop(IDropInfo dropInfo) {

      }
   }
}
