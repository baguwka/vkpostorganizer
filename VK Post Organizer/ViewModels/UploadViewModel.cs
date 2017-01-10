using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

      public string ProgressString {
         get { return _progressString; }
         set { SetProperty(ref _progressString, value); }
      }

      public WallControl Wall { get; private set; }

      public ICommand BrowseCommand { get; set; }

      public float UploadProgress {
         get { return _uploadProgress; }
         set { SetProperty(ref _uploadProgress, value); }
      }

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
      
      private string _urlOfImageToDownload;
      private string _progressString;
      private string _proxyServer;

      public UploadViewModel() {
         Files = new SmartCollection<string>();
         Wall = App.Container.Resolve<WallControl>();

         UploadCommand = new DelegateCommand(uploadCommandExecute);
         OpenFilesCommand = new DelegateCommand(openFilesCommandExecute);
         BrowseCommand = new DelegateCommand(browseCommandExecute);
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

      public string UrlOfImageToDownload {
         get { return _urlOfImageToDownload; }
         set {
            SetProperty(ref _urlOfImageToDownload, value);
            uploadImageIfPossible();
         }
      }

      public string ProxyServer {
         get { return _proxyServer; }
         set { SetProperty(ref _proxyServer, value); }
      }

      public bool IsImageContentType(string contentType) {
         return contentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/");
      }

      public async Task<string> GetContentType(string url) {
         try {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "HEAD";

            if (isUrlIsValid(ProxyServer)) {
               req.Proxy = new WebProxy(ProxyServer);
            }

            using (var response = await req.GetResponseAsync()) {
               return response.ContentType;
            }
         }
         catch (Exception ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            return "";
         }
      }

      private bool isUrlIsValid(string url) {
         if (!string.IsNullOrEmpty(url)) {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttp;

         }
         return false;
      }

      private async void uploadImageIfPossible() {
         var url = UrlOfImageToDownload;
         var contentType = await GetContentType(url);
         if (!IsImageContentType(contentType)) return;

         IsBusy = true;

         var ext = MimeTypeLibrary.GetExtension(contentType);
         var directory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Baguwk\\Vk Postpone Helper";

         try {
            if (!Directory.Exists(directory)) {
               Directory.CreateDirectory(directory);
            }
         }
         catch (IOException ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            filepath = "";
            IsBusy = false;
            return;
         }

         string fileLocation = $"{directory}\\tempImage.{ext}";

         try {
            using (var wc = new WebClient()) {
               if (isUrlIsValid(ProxyServer)) {
                  wc.Proxy = new WebProxy(ProxyServer);
               }

               wc.DownloadProgressChanged += onDownloadProgressChanged;
               await wc.DownloadFileTaskAsync(url, fileLocation);
            }
         }
         catch (Exception ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            filepath = "";
            IsBusy = false;
            return;
         }

         filepath = fileLocation;
         IsBusy = false;
      }

      private async void uploadCommandExecute() {
         if (IsBusy) return;

         IsBusy = true;
         try {
            var getUploadServerMethod = App.Container.Resolve<PhotosGetWallUploadSever>();
            var uploadServer = getUploadServerMethod.Get(-Wall.WallHolder.ID);

            if (!string.IsNullOrEmpty(filepath) || File.Exists(filepath)) {

               byte[] uploadBytes;
               using (var wc = new WebClient()) {
                  wc.UploadProgressChanged += onUploadProgressChanged;
                  uploadBytes = await wc.UploadFileTaskAsync(new Uri(uploadServer.UploadUrl), "POST", filepath);
               }

               if (uploadBytes == null) return;

               var uploadResponse = Encoding.UTF8.GetString(uploadBytes);
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

               Text = "";
               filepath = "";

               Messenger.Broadcast("refresh");
               getNextDate();

               IsBusy = false;
            }
         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message);
            IsBusy = false;
         }
      }

      private void onDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
         UploadProgress = e.ProgressPercentage;
         ProgressString = $"{SizeHelper.Suffix(e.BytesReceived)}/{SizeHelper.Suffix(e.TotalBytesToReceive)}";
      }

      private void onUploadProgressChanged(object sender, UploadProgressChangedEventArgs e) {
         UploadProgress = e.ProgressPercentage;
         ProgressString = $"{SizeHelper.Suffix(e.BytesSent)}/{SizeHelper.Suffix(e.TotalBytesToSend)}";
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
