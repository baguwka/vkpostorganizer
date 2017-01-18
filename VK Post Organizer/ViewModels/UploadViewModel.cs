using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
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
   [UsedImplicitly]
   public class UploadViewModel : BindableBase, IViewModel, IDropTarget {
      private string _text;
      private string _dateString;
      private int _dateUnix;
      private float _uploadProgress;
      private bool _isBusy;

      private string _urlOfImageToDownload;
      private string _progressString;
      private string _progressName;
      private string _imagePreviewUrl;

      private readonly CancellationTokenSource _cancellationToken;

      public ICommand PublishCommand { get; set; }
      public ICommand BrowseCommand { get; set; }

      public ICommand MovePreviousCommand { get; set; }
      public ICommand MoveNextCommand { get; set; }

      public SmartCollection<string> Files { get; set; }

      public string Text {
         get { return _text; }
         set { SetProperty(ref _text, value); }
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

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }

      public string UrlOfImageToDownload {
         get { return _urlOfImageToDownload; }
         set {
            SetProperty(ref _urlOfImageToDownload, value);
            downloadImageIfPossible(_urlOfImageToDownload);
         }
      }

      public WallControl Wall { get; private set; }

      public float UploadProgress {
         get { return _uploadProgress; }
         set { SetProperty(ref _uploadProgress, value); }
      }

      public SmartCollection<AttachmentItem> Attachments { get; }

      public string ImagePreviewUrl {
         get { return _imagePreviewUrl; }
         set { SetProperty(ref _imagePreviewUrl, value); }
      }

      public UploadViewModel() {
         _cancellationToken = new CancellationTokenSource();

         Attachments = new SmartCollection<AttachmentItem>();

         Files = new SmartCollection<string>();
         Wall = App.Container.GetInstance<WallControl>();

         PublishCommand = new DelegateCommand(publishCommandExecute);
         BrowseCommand = new DelegateCommand(browseCommandExecute);

         MovePreviousCommand = new DelegateCommand(moveToPreviousMissing);
         MoveNextCommand = new DelegateCommand(moveToNextMissing);
      }

      private void moveToPreviousMissing() {
         if (Wall.Items.None() || DateUnix <= 0) return;

         var previousOne = Wall.Items.LastOrDefault((p) => p.Post.DateUnix < DateUnix);
         if (previousOne == null) {
            previousOne = Wall.Items.LastOrDefault();
            if (previousOne == null) return;
            DateUnix = previousOne.Post.DateUnix;
         }
         else {
            DateUnix = previousOne.Post.DateUnix;
         }
      }

      private void moveToNextMissing() {
         if (Wall.Items.None() || DateUnix <= 0) return;

         var nextOne = Wall.Items.FirstOrDefault((p) => p.Post.DateUnix > DateUnix);
         if (nextOne == null) {
            nextOne = Wall.Items.FirstOrDefault();
            if (nextOne == null) return;
            DateUnix = nextOne.Post.DateUnix;
         }
         else {
            DateUnix = nextOne.Post.DateUnix;
         }
      }

      private void refresh() {
         Wall.PullWithScheduleHightlight(new MissingPostFilter(), new Schedule());
      }

      public void Configure(UploadInfo info) {
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

      private async void browseCommandExecute() {
         var openFile = new OpenFileDialog {Multiselect = true};
         var checker = App.Container.GetInstance<ImageExtensionChecker>();

         openFile.Filter = checker.GetFileFilter();
         var result = openFile.ShowDialog();
         if (result == true) {
            var files = openFile.FileNames.Take(10);
            foreach (var file in files.Where(file => checker.IsFileHaveValidExtension(file))) {
               if (_cancellationToken.IsCancellationRequested) break;

               await tryToUpload(file);
            }
         }
      }

      private async Task tryToUpload(string filePath) {
         if (Attachments.Count >= 10) {
            //MessageBox.Show("Only 10 attachments allowed", "Too much attachments", MessageBoxButton.OK,MessageBoxImage.Error);
            return;
         }

         IsBusy = true;
         var photoInfo = await tryToUploadPhoto(filePath);

         if (photoInfo.Successful) {
            addPhotoToAttachments(photoInfo.Result);
            ImagePreviewUrl = photoInfo.Result.Photo130;
         }

         IsBusy = false;
      }

      public async Task ImportFilesAsync(IEnumerable<string> files) {
         foreach (var file in files.Take(10)) {
            if (_cancellationToken.IsCancellationRequested) break;

            if (Attachments.Count > 10) continue;

            if (File.Exists(file) && App.Container.GetInstance<ImageExtensionChecker>().IsFileHaveValidExtension(file)) {

               var src = new BitmapImage();
               src.BeginInit();
               src.CacheOption = BitmapCacheOption.OnLoad;
               src.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
               src.UriSource = new Uri(file, UriKind.Absolute);
               src.EndInit();
            }

            await tryToUpload(file);
         }
      }

      public bool IsImageContentType(string contentType) {
         return contentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/");
      }

      /// <summary>
      /// 
      /// </summary>
      public async Task<string> GetContentType(string url) {
         var req = (HttpWebRequest)WebRequest.Create(url);
         req.Proxy = null;
         req.Method = "HEAD";

         var settings = App.Container.GetInstance<Settings>();
         if (settings.Proxy.UseProxy) {
            var myProxy = App.Container.GetInstance<ProxyProvider>().GetProxy();
            if (myProxy != null) {
               req.Proxy = myProxy;
            }
         }

         using (var response = await req.GetResponseAsync()) {
            return response.ContentType;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private async void downloadImageIfPossible(string url) {
         if (string.IsNullOrEmpty(url)) return;
         if (UrlHelper.IsUrlIsValid(url) == false) return;

         IsBusy = true;

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
         var directory =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Baguwk\\Vk Postpone Helper";

         try {
            if (!Directory.Exists(directory)) {
               Directory.CreateDirectory(directory);
            }
         }
         catch (IOException ex) {
            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            IsBusy = false;
            return;
         }

         string fileLocation = $"{directory}\\tempImage.{ext}";

         try {
            using (var wc = new WebClient()) {
               var settings = App.Container.GetInstance<Settings>();
               if (settings.Proxy.UseProxy) {
                  var myProxy = App.Container.GetInstance<ProxyProvider>().GetProxy();
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
            IsBusy = false;
            return;
         }

         var src = new BitmapImage();
         src.BeginInit();
         src.CacheOption = BitmapCacheOption.OnLoad;
         src.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
         src.UriSource = new Uri(fileLocation, UriKind.Absolute);
         src.EndInit();

         IsBusy = false;
         UrlOfImageToDownload = string.Empty;

         await tryToUpload(fileLocation);
      }


      [ItemNotNull]
      private async Task<UploadPhotoInfo> tryToUploadPhoto(string filePath) {
         try {
            var getUploadServerMethod = App.Container.GetInstance<PhotosGetWallUploadSever>();
            var uploadServer = getUploadServerMethod.Get(-Wall.WallHolder.ID);

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) {
               return new UploadPhotoInfo(null, false);
            }

            byte[] uploadBytes;
            using (var wc = new WebClient()) {
               wc.UploadProgressChanged += onUploadProgressChanged;
               uploadBytes = await wc.UploadFileTaskAsync(new Uri(uploadServer.UploadUrl), "POST", filePath);
            }

            if (uploadBytes == null) {
               return new UploadPhotoInfo(null, false);
            }

            var uploadResponse = Encoding.UTF8.GetString(uploadBytes);

            if (string.IsNullOrEmpty(uploadResponse)) {
               return new UploadPhotoInfo(null, false);
            }

            var savePhotoMethod = App.Container.GetInstance<PhotosSaveWallPhoto>();
            var savePhotoProperty = savePhotoMethod.Save(-Wall.WallHolder.ID, uploadResponse);

            var savedPhoto = savePhotoProperty?.Response.FirstOrDefault();

            return new UploadPhotoInfo(savedPhoto, savedPhoto != null);
         }
         catch (VkException ex) {
            MessageBox.Show($"{ex.Message}\n\nStackTrace:\n{ex.StackTrace}", ex.ToString(), MessageBoxButton.OK,
               MessageBoxImage.Error);
            return new UploadPhotoInfo(null, false);
         }
      }

      private void addPhotoToAttachments([NotNull] Photo photo) {
         if (photo == null) {
            throw new ArgumentNullException(nameof(photo));
         }

         var attachment = new AttachmentItem();
         attachment.Set("photo", photo);
         Attachments.Add(attachment);
         attachment.RemoveRequested += onAttachmentRemoveRequest;
      }

      private void onAttachmentRemoveRequest(object sender, EventArgs eventArgs) {
         var attachment = sender as AttachmentItem;
         if (attachment == null) return;

         Attachments.Remove(attachment);
         attachment.RemoveRequested -= onAttachmentRemoveRequest;
      }

      private async void publishCommandExecute() {
         if (IsBusy) return;

         IsBusy = true;
         try {
            var wallPostMethod = App.Container.GetInstance<WallPost>();
            await wallPostMethod.PostAsync(-Wall.WallHolder.ID, Text, false, true, DateUnix,
               Attachments.Take(10).Select(item => item.Attachment));
         }
         catch (VkException ex) {
            MessageBox.Show($"{ex.Message}\n\nStackTrace:\n{ex.StackTrace}", ex.ToString(), MessageBoxButton.OK,
               MessageBoxImage.Error);
         }
         finally {
            Text = "";
            Attachments.Clear();

            Messenger.Broadcast("refresh");
            refresh();
            moveToNextMissing();

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

      public void InterruptAllWork() {
         _cancellationToken.Cancel();
      }
   }
}
