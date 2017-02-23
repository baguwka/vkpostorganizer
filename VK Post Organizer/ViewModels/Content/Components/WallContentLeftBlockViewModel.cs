using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;
using vk.Models;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallContentLeftBlockViewModel : BindableBase, INavigationAware {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private readonly VkUploader _uploader;
      private readonly VkApiProvider _vkApi;
      private string _description;
      private string _profilePhoto;
      private string _name;
      private string _infoPanel;

      public ICommand ShowActualWallCommand { get; private set; }
      public ICommand ShowPostponeWallCommand { get; private set; }
      public ICommand ShowHistoryCommand { get; private set; }
      public ICommand ExpandAllCommand { get; private set; }
      public ICommand CollapseAllCommand { get; private set; }

      public string Description {
         get { return _description; }
         set { SetProperty(ref _description, value); }
      }

      public string ProfilePhoto {
         get { return _profilePhoto; }
         set { SetProperty(ref _profilePhoto, value); }
      }

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public string InfoPanel {
         get { return _infoPanel; }
         set { SetProperty(ref _infoPanel, value); }
      }

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,
         VkUploader uploader, VkApiProvider vkApi) {
         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
         _uploader = uploader;
         _vkApi = vkApi;

         ShowActualWallCommand = new DelegateCommand(() => {
            var parameters = new NavigationParameters {{"filter", "howdy"}};
            _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallActualContent, parameters);
         });

         ShowPostponeWallCommand =
            DelegateCommand.FromAsyncHandler(
               async () => {
                  _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                     $"{ViewNames.WallPostponeContent}?filter=sayhello");
                  try {
                     var list = new List<string>();
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.UsersGet.GetAsync();
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.UsersGet.GetAsync();
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.UsersGet.GetAsync();
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.UsersGet.GetAsync();
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.UsersGet.GetAsync();
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.WallGet.GetAsync(-127092063);
                     await _vkApi.GroupsGetById.GetAsync(-127092063);
                     await _vkApi.UsersGet.GetAsync();
                     await _vkApi.WallGet.GetAsync(-127092063);
                     var another = list.ToList();
                     MessageBox.Show(another.Count.ToString());
                  }
                  catch (VkException ex) {
                     MessageBox.Show(ex.Message);
                  }
               });

         ShowHistoryCommand =
            new DelegateCommand(
               () => {
                  _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                     $"{ViewNames.HistoryContent}?filter=sayhello");
               });
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