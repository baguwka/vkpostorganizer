using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using vk.Views;

namespace vk.ViewModels {
   public class UploadViewModel : BindableBase, IViewModel, IDropTarget {
      private string _text;
      private string _attachment1;
      private string _attachment2;
      private string _filepath;
      private string _dateString;
      public ICommand UploadCommand { get; set; }
      public ICommand OpenFilesCommand { get; set; }

      public SmartCollection<string> Files { get; set; }

      public string Text {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }

      public string Attachment1 {
         get { return _attachment1; }
         set { SetProperty(ref _attachment1, value); }
      }

      public string Attachment2 {
         get { return _attachment2; }
         set { SetProperty(ref _attachment2, value); }
      }

      public string filepath {
         get { return _filepath; }
         set { SetProperty(ref _filepath, value); }
      }

      public string DateString {
         get { return _dateString; }
         set { SetProperty(ref _dateString, value); }
      }

      public WallControl Wall { get; private set; }

      public void PrepareImages(UploadInfo info) {
         Files.Clear();
         Files.AddRange(info.Files);

         Wall.WallHolder = info.Wall.WallHolder;
         Wall.PullWithScheduleHightlight(new MissingPostFilter(), new Schedule());

         var firstMissed = Wall.Items.FirstOrDefault((p) => p.PostType == PostType.Missing);

         if (firstMissed == null) return;
         DateString = firstMissed.Post.DateString;
      }

      public void ImportFiles(IEnumerable<string> files) {
         filepath = files.First();
         //var filter = new FileFilter(new ImageExtensionChecker());
         //Files.AddRange(filter.FilterOut(files));
      }

      public UploadViewModel() {
         Files = new SmartCollection<string>();
         Wall = App.Container.Resolve<WallControl>();

         UploadCommand = new DelegateCommand(uploadCommandExecute);

         OpenFilesCommand = new DelegateCommand(openFilesCommand);
      }

      private void uploadCommandExecute() {
         var attachments = new List<string>();
         try {
            var getUploadServerMethod = App.Container.Resolve<PhotosGetWallUploadSever>();
            var savePhotoMethod = App.Container.Resolve<PhotosSaveWallPhoto>();
            var wallPostMethod = App.Container.Resolve<WallPost>();

            var uploadServer = getUploadServerMethod.Get(-Wall.WallHolder.ID);

            if (!string.IsNullOrEmpty(filepath) || File.Exists(filepath)) {
               var savedphoto = savePhotoMethod.Save(-Wall.WallHolder.ID, uploadServer.UploadUrl, filepath);
               attachments.Add($"photo{uploadServer.UserId}_{savedphoto.Response[0].Id}");
            }

            var firstMissed = Wall.Items.FirstOrDefault((p)=>p.PostType == PostType.Missing);
            if (firstMissed != null) {
               var date = firstMissed.Post.DateUnix;

               wallPostMethod.Post(-Wall.WallHolder.ID, Text, false, true, date, attachments);

               Text = "";
               filepath = "";
               Messenger.Broadcast("refresh");
               Wall.PullWithScheduleHightlight(new MissingPostFilter(), new Schedule());
               firstMissed = Wall.Items.FirstOrDefault((p) => p.PostType == PostType.Missing);
               if (firstMissed == null) return;
               DateString = firstMissed.Post.DateString;
            }
         }
         catch (VkException ex) {
            MessageBox.Show(ex.Message);
         }
      }

      private void openFilesCommand() {
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


      //if (TestingGroup != Wall.WallHolder.ID) {
      //   var groupsGet = App.Container.Resolve<GroupsGetById>();
      //   var response = groupsGet.Get(TestingGroup);
      //   var group = response.Response.FirstOrDefault();

      //   MessageBox.Show($"You're only available to post in \"{group?.Name}\" wall in testing purposes.", "Cant post here",
      //      MessageBoxButton.OK, MessageBoxImage.Error);
      //   return;
      //}

      //var wallPost = App.Container.Resolve<WallPost>();

      //try {
      //   var date = new DateTime(2016, 12, 22, 18, 30, 0);
      //   for (int i = 0; i < 150; i++) {
      //      date = date.AddHours(1);
      //      var unixTimestamp = UnixTimeConverter.ToUnix(date);
      //      var post = wallPost.Post(Wall.WallHolder.ID, $"Тестовая пустая отложка номер {i}", false, true, unixTimestamp);
      //   }

      //   refreshCommandExecute();
      //}
      //catch (VkException ex) {
      //   MessageBox.Show(ex.Message);
      //}
   }

   public class UploadSlotItem {
      
   }
}
