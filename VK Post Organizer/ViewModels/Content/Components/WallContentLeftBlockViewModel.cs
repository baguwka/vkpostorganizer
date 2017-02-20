using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;
using vk.Models;
using vk.Views;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallContentLeftBlockViewModel : BindableBase, INavigationAware {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private readonly VkUploader _uploader;

      public ICommand ShowActualWallCommand { get; private set; }
      public ICommand ShowPostponeWallCommand { get; private set; }
      public ICommand ShowHistoryCommand { get; private set; }

      public ICommand CancelCommand { get; private set; }


      private CancellationTokenSource cts;

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, VkUploader uploader) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
         _uploader = uploader;

         cts = new CancellationTokenSource();

         ShowActualWallCommand = new DelegateCommand(() => {
            var parameters = new NavigationParameters {{"filter", "howdy"}};
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallActualContent, parameters);
         });

         ShowPostponeWallCommand = new DelegateCommand(() => {
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, $"{ViewNames.WallPostponeContent}?filter=sayhello");
         });

         ShowHistoryCommand = new DelegateCommand(() => {
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, $"{ViewNames.HistoryContent}?filter=sayhello");
         });

         CancelCommand = new DelegateCommand(() => {
            cts.Cancel();
         });
      }

      private async void somemethod(Uri uri) {
         cts = new CancellationTokenSource();

         var progress = new Progress<int>();
         progress.ProgressChanged += onProgressChanged;

         var downloadResult = await _uploader.DownloadPhotoByUriAsync(uri, progress, cts.Token);

         if (!downloadResult.Successful) {
            if (!cts.IsCancellationRequested) {
               MessageBox.Show(downloadResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return;
         }

         var content = new MultipartFormDataContent();
         var file = new ByteArrayContent(downloadResult.Photo);
         file.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") {
            Name = "file",
            FileName = "file.jpg"
         };
         content.Add(file);

         var result = await _uploader.TryUploadPhotoToWallAsync(content, -127092063, progress, cts.Token);
         if (result.Successful) {
            ResultUrl = result.Result?.GetLargest();
         }
         else {
            if (!cts.IsCancellationRequested) {
               MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return;
         }
      }

      private string _progressString;

      public string ProgressString {
         get { return _progressString; }
         set { SetProperty(ref _progressString, value); }
      }

      private void onProgressChanged(object sender, int i) {
         ProgressString = i.ToString();
      }

      private string _urlToDownload;

      public string UrlToDownload {
         get { return _urlToDownload; }
         set {
            SetProperty(ref _urlToDownload, value);
            somemethod(new Uri(_urlToDownload));
         }
      }

      private string _resultUrl;

      public string ResultUrl {
         get { return _resultUrl; }
         set { SetProperty(ref _resultUrl, value); }
      }



      public void OnNavigatedTo(NavigationContext navigationContext) {
      }

      public bool IsNavigationTarget(NavigationContext navigationContext) {
         return true;
      }

      public void OnNavigatedFrom(NavigationContext navigationContext) {
      }
   }
}