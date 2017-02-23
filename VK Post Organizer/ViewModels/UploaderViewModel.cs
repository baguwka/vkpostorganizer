using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
using vk.Models;
using vk.Models.Files;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   public class UploaderViewModelConfiguration {
      public bool IsEnabled { get; set; }
      public int WallId { get; set; }
      public int DateOverride { get; set; }
   }

   [UsedImplicitly]
   public class UploaderViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly VkUploader _uploader;
      private readonly UploadSettings _uploadSettings;
      private readonly VkApiProvider _vkApi;
      private readonly WallContainer _wallContainer;
      private ObservableCollection<AttachmentItem> _attachments;

      private CancellationTokenSource _cts;

      private bool _isEnabled;
      private bool _isBusy;
      private string _urlOfImageToUpload;
      private string _message;
      private bool _isShowing;
      private bool _shrinkAfterPublish;
      private float _uploadProgress;
      private string _progressString;
      private string _dateString;
      private int _dateUnix;
      private string _wallName;
      private string _infoPanel;

      public bool IsEnabled {
         get { return _isEnabled; }
         set { SetProperty(ref _isEnabled, value); }
      }

      public bool IsBusy {
         get { return _isBusy; }
         set { SetProperty(ref _isBusy, value); }
      }

      public string Message {
         get { return _message; }
         set { SetProperty(ref _message, value); }
      }

      public string UrlOfImageToUpload {
         get { return _urlOfImageToUpload; }
         set {
            SetProperty(ref _urlOfImageToUpload, value);

            onUrlOfImageToUploadChanged(_urlOfImageToUpload);
         }
      }

      public bool ShrinkAfterPublish {
         get { return _shrinkAfterPublish; }
         set {
            SetProperty(ref _shrinkAfterPublish, value);
            _uploadSettings.CloseUploadWindowAfterPublish = _shrinkAfterPublish;
         }
      }

      public bool IsShowing {
         get { return _isShowing; }
         set { SetProperty(ref _isShowing, value); }
      }

      public float UploadProgress {
         get { return _uploadProgress; }
         set { SetProperty(ref _uploadProgress, value); }
      }

      public string ProgressString {
         get { return _progressString; }
         set { SetProperty(ref _progressString, value); }
      }

      public ObservableCollection<AttachmentItem> Attachments {
         get { return _attachments; }
         set { SetProperty(ref _attachments, value); }
      }

      public int DateUnix {
         get { return _dateUnix; }
         set {
            SetProperty(ref _dateUnix, value);
            DateString = UnixTimeConverter.ToDateTime(_dateUnix).ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture);
         }
      }

      public string DateString {
         get { return _dateString; }
         set { SetProperty(ref _dateString, value); }
      }

      public string WallName {
         get { return _wallName; }
         set { SetProperty(ref _wallName, value); }
      }

      public string InfoPanel {
         get { return _infoPanel; }
         set { SetProperty(ref _infoPanel, value); }
      }

      public ICommand ShowHideCommand { get; private set; }
      public ICommand PublishCommand { get; private set; }
      public ICommand CancelCommand { get; private set; }
      public ICommand WipeCommand { get; private set; }
      public ICommand BrowseCommand { get; private set; }
      public ICommand MovePreviousCommand { get; private set; }
      public ICommand MoveNextCommand { get; private set; }


      public UploaderViewModel(IEventAggregator eventAggregator, VkUploader uploader, UploadSettings uploadSettings, VkApiProvider vkApi, WallContainer wallContainer) {
         _eventAggregator = eventAggregator;
         _uploader = uploader;
         _uploadSettings = uploadSettings;
         _vkApi = vkApi;
         _wallContainer = wallContainer;

         _wallContainer.PullInvoked += (sender, args) => {
            IsBusy = true;
            ProgressString = "Pull...";
         };

         _wallContainer.PullCompleted += (sender, args) => {
            var missing = _wallContainer.GetMissingPostCount();
            InfoPanel = $"{WallContainer.MAX_POSTPONED - missing}/{WallContainer.MAX_POSTPONED}";
            IsBusy = false;
            ProgressString = "";
         };

         _shrinkAfterPublish = _uploadSettings.CloseUploadWindowAfterPublish;

         Attachments = new ObservableCollection<AttachmentItem>();

         _cts = new CancellationTokenSource();

         CancelCommand = new DelegateCommand(() => {
            _cts.Cancel();
         }, () => IsShowing)
         .ObservesProperty(() => IsShowing);

         ShowHideCommand = new DelegateCommand(() => {
            IsShowing = !IsShowing;
         });

         PublishCommand = DelegateCommand.FromAsyncHandler(publishExecute, 
            () => IsEnabled && IsShowing && !IsBusy && (!string.IsNullOrWhiteSpace(Message) || Attachments.Any()))
         .ObservesProperty(() => IsShowing)
         .ObservesProperty(() => IsBusy)
         .ObservesProperty(() => Message)
         .ObservesProperty(() => Attachments)
         .ObservesProperty(() => IsEnabled);

         BrowseCommand = DelegateCommand.FromAsyncHandler(browseExecute, () => IsShowing)
            .ObservesProperty(() => IsShowing);

         WipeCommand = new DelegateCommand(wipeExecute, () => IsShowing )
         //.ObservesProperty(() => IsBusy)
         .ObservesProperty(() => IsShowing);

         MovePreviousCommand = new DelegateCommand(moveToPreviousMissing);
         MoveNextCommand = new DelegateCommand(moveToNextMissing);

         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(async () => {
            if (IsBusy || !IsEnabled) return;
            IsBusy = true;
            ProgressString = "Pull...";
            try {
               await _wallContainer.PullWithScheduleHightlightAsync(new MissingPostFilter(), new Schedule());
            }
            finally {
               IsBusy = false;
               ProgressString = "";
            }
         });

         _eventAggregator.GetEvent<UploaderEvents.SetVisibility>().Subscribe(onSetVisibility);
         _eventAggregator.GetEvent<UploaderEvents.Configure>().Subscribe(onConfigure);
      }

      private void onSetVisibility(bool visibility) {
         IsShowing = visibility;
      }

      private async void onConfigure(UploaderViewModelConfiguration config) {
         //todo: как то это не очень.
         if (IsBusy) {
            _cts.Cancel();
            MessageBox.Show("Все задачи Uploader'а были отменены", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
         }

         IsEnabled = config.IsEnabled;

         if (!config.IsEnabled) {
            return;
         }

         _wallContainer.WallHolder = new WallHolder(config.WallId);
         var thisGroup = await _vkApi.GroupsGetById.GetAsync(config.WallId);
         WallName = thisGroup.Response.FirstOrDefault()?.Name;

         await _wallContainer.PullWithScheduleHightlightAsync(new MissingPostFilter(), new Schedule());

         if (config.DateOverride == -1) {
            var firstMissed = _wallContainer.Items.FirstOrDefault();
            if (firstMissed == null) {
               return;
            }
            DateUnix = firstMissed.Post.DateUnix;
         }
         else {
            DateUnix = config.DateOverride;
         }
      }


      private bool canUpload() {
         return Attachments.Count < 10;
      }

      public void moveToPreviousMissing() {
         if (_wallContainer.Items.None() || DateUnix <= 0) {
            return;
         }

         var previousOne = _wallContainer.Items.LastOrDefault(p => p.Post.DateUnix < DateUnix);
         if (previousOne == null) {
            previousOne = _wallContainer.Items.LastOrDefault();
            if (previousOne == null) {
               return;
            }
            DateUnix = previousOne.Post.DateUnix;
         }
         else {
            DateUnix = previousOne.Post.DateUnix;
         }
      }

      public void moveToNextMissing() {
         if (_wallContainer.Items.None() || DateUnix <= 0) {
            return;
         }

         var nextOne = _wallContainer.Items.FirstOrDefault(p => p.Post.DateUnix > DateUnix);
         if (nextOne == null) {
            nextOne = _wallContainer.Items.FirstOrDefault();
            if (nextOne == null) {
               return;
            }
            DateUnix = nextOne.Post.DateUnix;
         }
         else {
            DateUnix = nextOne.Post.DateUnix;
         }
      }

      private async Task browseExecute() {
         if (!canUpload()) return;

         var openFile = new OpenFileDialog { Multiselect = true };
         var checker = new ImageExtensionChecker();

         _cts = new CancellationTokenSource();
         openFile.Filter = checker.GetFileFilter();
         var result = openFile.ShowDialog();
         if (result == true) {
            var files = openFile.FileNames.Take(10);
            foreach (var file in files.Where(file => checker
                  .IsFileHaveValidExtension(file))
                  .TakeWhile(file => !_cts.IsCancellationRequested)) {

               await doBusyWork(tryToUploadFromFile(file, _cts.Token));
            }
         }
      }

      //todo: async void because of property, do something with it
      private async void onUrlOfImageToUploadChanged(string urlOfImageToUpload) {
         if (!canUpload()) return;
         if (string.IsNullOrEmpty(urlOfImageToUpload)) {
            return;
         }
         if (UrlHelper.IsUrlIsValid(urlOfImageToUpload) == false) {
            return;
         }

         _cts = new CancellationTokenSource();
         await doBusyWork(tryToUploadImageFromUri(new Uri(urlOfImageToUpload), _cts.Token));
      }

      private async Task tryToUploadFromFile(string filePath, CancellationToken cancellationToken) {
         if (!canUpload()) return;
         if (!File.Exists(filePath)) {
            return;
         }

         var image = await Task.Run(() =>File.ReadAllBytes(filePath), cancellationToken);
         await uploadFromBytes(image, cancellationToken);
      }

      private async Task tryToUploadImageFromUri(Uri uri, CancellationToken cancellationToken) {
         var progress = new Progress<int>();
         progress.ProgressChanged += onProgressChanged;

         ProgressString = "Downloading...";
         UploadProgress = 0;

         var downloadResult = await _uploader.DownloadPhotoByUriAsync(uri, progress, cancellationToken);

         if (!downloadResult.Successful) {
            if (!_cts.IsCancellationRequested) {
               MessageBox.Show(downloadResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
               UrlOfImageToUpload = string.Empty;
            }
            ProgressString = string.Empty;
            return;
         }

         ProgressString = string.Empty;
         await uploadFromBytes(downloadResult.Photo, cancellationToken);
         UrlOfImageToUpload = string.Empty;
      }

      private async Task doBusyWork(Task task) {
         try {
            IsBusy = true;
            await task;
         }
         finally {
            IsBusy = false;
         }
      }

      private async Task uploadFromBytes(byte[] photo, CancellationToken cancellationToken) {
         ProgressString = "Uploading...";
         UploadProgress = 100;
         var result = await _uploader.TryUploadPhotoToWallAsync(photo, _wallContainer.WallHolder.ID, cancellationToken);
         if (result.Successful) {
            addPhotoToAttachments(result.Photo);
         }
         else {
            if (!_cts.IsCancellationRequested) {
               MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ProgressString = string.Empty;
            return;
         }
         ProgressString = string.Empty;
      }

      private void onProgressChanged(object sender, int e) {
         UploadProgress = e;
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

      private void onAttachmentRemoveRequest(object sender, EventArgs e) {
         var attachment = sender as AttachmentItem;
         if (attachment == null) {
            return;
         }

         Attachments.Remove(attachment);
         attachment.RemoveRequested -= onAttachmentRemoveRequest;
      }

      private async Task publishExecute() {
         IsBusy = true;
         try {
            await _vkApi.WallPost.PostAsync(-_wallContainer.WallHolder.ID, Message, false, true, DateUnix,
               Attachments.Take(10).Select(item => item.Attachment));
         }
         catch (VkException ex) {
            // 150 postpone posts reached (probably)
            if (ex.ErrorCode == 214) {
               MessageBox.Show(ex.Message, "Невозможно отложить пост",
                  MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else {
               MessageBox.Show($"{ex.Message}\n\nStackTrace:\n{ex.StackTrace}", ex.ToString(), MessageBoxButton.OK,
                  MessageBoxImage.Error);
            }
         }
         finally {
            wipe();
            if (ShrinkAfterPublish) {
               IsShowing = false;
            }
            _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Publish();
            IsBusy = false;
         } 
      }

      private void wipeExecute() {
         var result = MessageBox.Show("Стереть вложения?", "Caution", MessageBoxButton.YesNo, MessageBoxImage.Question);
         if (result == MessageBoxResult.Yes) {
            wipe();
         }
      }

      private void wipe() {
         Message = string.Empty;
         UrlOfImageToUpload = string.Empty;
         Attachments.Clear();
      }
   }
}