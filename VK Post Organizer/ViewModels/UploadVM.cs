using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using vk.Models;
using vk.Models.Files;
using vk.Views;

namespace vk.ViewModels {
   public class UploadVM : BindableBase, IVM, IDropTarget {
      public ICommand UploadCommand { get; set; }
      public ICommand OpenFilesCommand { get; set; }

      public SmartCollection<PostControl> Slots { get; set; }
      public SmartCollection<string> Files { get; set; }

      public void PrepareImages(UploadInfo info) {
         Slots.Clear();
         Slots.AddRange(info.Slots);

         Files.Clear();
         Files.AddRange(info.Files);
      }

      public void ImportFiles(IEnumerable<string> files) {
         var filter = new FileFilter(new ImageExtensionChecker());
         Files.AddRange(filter.FilterOut(files));
      }

      public UploadVM() {
         Slots = new SmartCollection<PostControl>();
         Files = new SmartCollection<string>();

         UploadCommand = new DelegateCommand(() => {
            
         });

         OpenFilesCommand = new DelegateCommand(openFilesCommand);
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
}
