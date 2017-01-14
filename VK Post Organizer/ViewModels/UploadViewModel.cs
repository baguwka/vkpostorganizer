﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

      public string Filepath {
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

      public string ProgressName {
         get { return _progressName; }
         set { SetProperty(ref _progressName, value); }
      }

      public string ProgressString {
         get { return _progressString; }
         set {
            if (value == _progressName) return;
            SetProperty(ref _progressString, value);
         }
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
         Filepath = files.First();

         if (File.Exists(Filepath) && App.Container.Resolve<ImageExtensionChecker>().IsFileHaveValidExtension(Filepath)) {
            var src = new BitmapImage();
            src.BeginInit();
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            src.UriSource = new Uri(Filepath, UriKind.Absolute);
            src.EndInit();
            ImagePreview = src;
         }
         //var filter = new FileFilter(new ImageExtensionChecker());
         //Files.AddRange(filter.FilterOut(files));
      }
      
      private string _urlOfImageToDownload;
      private string _progressString;
      private string _progressName;
      private ImageSource _imagePreview;

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
               Filepath = file;
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

      public ImageSource ImagePreview {
         get { return _imagePreview; }
         set { SetProperty(ref _imagePreview, value); }
      }

      public bool IsImageContentType(string contentType) {
         return contentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/");
      }

      public async Task<string> GetContentType(string url) {
         var req = (HttpWebRequest)WebRequest.Create(url);
         req.Proxy = null;
         req.Method = "HEAD";

         var settings = App.Container.Resolve<Settings>();
         if (settings.Proxy.UseProxy) {
            var myProxy = App.Container.Resolve<ProxyProvider>().GetProxy();
            if (myProxy != null) {
               req.Proxy = myProxy;
            }
         }

         using (var response = await req.GetResponseAsync()) {
            return response.ContentType;
         }
      }

      private async void uploadImageIfPossible() {
         if (string.IsNullOrEmpty(UrlOfImageToDownload)) return;
         if (UrlHelper.IsUrlIsValid(UrlOfImageToDownload) == false) return;

         var url = UrlOfImageToDownload;
         IsBusy = true;

         Filepath = "";
         ImagePreview = null;

         string contentType;

         try {
            ProgressName = "Recieving image info...";
            contentType = await GetContentType(url);

            if (!IsImageContentType(contentType)) {
               IsBusy = false;
               return;
            }
         }
         catch (Exception ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            IsBusy = false;
            return;
         }

         var ext = MimeTypeLibrary.GetExtension(contentType);
         var directory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Baguwk\\Vk Postpone Helper";

         try {
            if (!Directory.Exists(directory)) {
               Directory.CreateDirectory(directory);
            }
         }
         catch (IOException ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            Filepath = "";
            ImagePreview = null;
            IsBusy = false;
            return;
         }

         string fileLocation = $"{directory}\\tempImage.{ext}";

         try {
            using (var wc = new WebClient()) {
               var settings = App.Container.Resolve<Settings>();
               if (settings.Proxy.UseProxy) {
                  var myProxy = App.Container.Resolve<ProxyProvider>().GetProxy();
                  if (myProxy != null) {
                     wc.Proxy = myProxy;
                  }
               }

               wc.DownloadProgressChanged += onDownloadProgressChanged;
               await wc.DownloadFileTaskAsync(url, fileLocation);
            }
         }
         catch (Exception ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            Filepath = "";
            ImagePreview = null;
            IsBusy = false;
            return;
         }

         Filepath = fileLocation;

         var src = new BitmapImage();
         src.BeginInit();
         src.CacheOption = BitmapCacheOption.OnLoad;
         src.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
         src.UriSource = new Uri(fileLocation, UriKind.Absolute);
         src.EndInit();
         ImagePreview = src;

         IsBusy = false;
         UrlOfImageToDownload = string.Empty;
      }

      private async void uploadCommandExecute() {
         if (IsBusy) return;

         IsBusy = true;
         try {
            var getUploadServerMethod = App.Container.Resolve<PhotosGetWallUploadSever>();
            var uploadServer = getUploadServerMethod.Get(-Wall.WallHolder.ID);

            var attachments = new List<string>();
            var wallPostMethod = App.Container.Resolve<WallPost>();

            if (!string.IsNullOrEmpty(Filepath) || File.Exists(Filepath)) {

               byte[] uploadBytes;
               using (var wc = new WebClient()) {
                  wc.UploadProgressChanged += onUploadProgressChanged;
                  uploadBytes = await wc.UploadFileTaskAsync(new Uri(uploadServer.UploadUrl), "POST", Filepath);
               }

               if (uploadBytes == null) return;

               var uploadResponse = Encoding.UTF8.GetString(uploadBytes);

               if (!string.IsNullOrEmpty(uploadResponse)) {
                  var savePhotoMethod = App.Container.Resolve<PhotosSaveWallPhoto>();
                  var savePhotoProperty = savePhotoMethod.Save(-Wall.WallHolder.ID, uploadResponse);

                  var result = savePhotoProperty;
                  if (result != null) {
                     var userId = App.Container.Resolve<AccessToken>().UserID;
                     attachments.AddRange(result.Response.Select(photoResponse => $"photo{userId}_{photoResponse.Id}"));
                  }
               }
            }

            wallPostMethod.Post(-Wall.WallHolder.ID, Text, false, true, DateUnix, attachments);
         }
         catch (VkException ex) {
            MessageBox.Show($"{ex.Message}\n\nStackTrace:\n{ex.StackTrace}", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
         }
         finally {
            Text = "";
            Filepath = "";
            ImagePreview = null;

            Messenger.Broadcast("refresh");
            getNextDate();

            IsBusy = false;
         }
      }

      private void updateProgress(int percentage, long current, long total) {
         UploadProgress = percentage;
         ProgressString = $"{SizeHelper.Suffix(current)}/{SizeHelper.Suffix(total)}";
      }

      private void onDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
         ProgressName = "Downloading...";
         updateProgress(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
      }

      private void onUploadProgressChanged(object sender, UploadProgressChangedEventArgs e) {
         ProgressName = "Uploading...";
         updateProgress(e.ProgressPercentage * 2, e.BytesSent, e.TotalBytesToSend);
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
