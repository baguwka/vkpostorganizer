﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using vk.Infrastructure;
using vk.Models;
using vk.Models.VkApi;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallContentLeftBlockViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private readonly IRegionManager _regionManager;
      private readonly VkApiProvider _vkApi;
      private readonly WallContainerController _wallContainerController;
      private readonly BusyObserver _busyObserver;
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

      private bool _contentIsBusy;

      public bool ContentIsBusy {
         get { return _contentIsBusy; }
         private set { SetProperty(ref _contentIsBusy, value); }
      }

      public WallContentLeftBlockViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,
         VkApiProvider vkApi, WallContainerController wallContainerController, BusyObserver busyObserver) {

         _eventAggregator = eventAggregator;
         _regionManager = regionManager;
         _vkApi = vkApi;
         _wallContainerController = wallContainerController;
         _busyObserver = busyObserver;
         _busyObserver.PropertyChanged += (sender, args) => {
            ContentIsBusy = _busyObserver.ContentIsBusy;
         };

         _wallContainerController.Container.WallHolderChanged += onWallContainerWallHolderChanged;
         _wallContainerController.Container.PullCompleted += onWallContainerPullCompleted;

         ShowActualWallCommand =
            new DelegateCommand(
                  () => {
                     var parameters = new NavigationParameters { { "filter", "howdy" } };
                     _regionManager.RequestNavigate(RegionNames.ContentMainRegion, ViewNames.WallActualContent,
                        parameters);
                  }, () => !ContentIsBusy)
               .ObservesProperty(() => ContentIsBusy);

         ShowPostponeWallCommand =
            new DelegateCommand(
                  () => {
                     _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                        $"{ViewNames.WallPostponeContent}?filter=sayhello");
                  }, () => !ContentIsBusy)
               .ObservesProperty(() => ContentIsBusy);

         ShowHistoryCommand =
            new DelegateCommand(
                  () => {
                     _regionManager.RequestNavigate(RegionNames.ContentMainRegion,
                        $"{ViewNames.HistoryContent}?filter=sayhello");
                  }, () => !ContentIsBusy)
               .ObservesProperty(() => ContentIsBusy);
      }

      private async void onWallContainerWallHolderChanged(object sender, IWallHolder wallHolder) {
         var response = await _vkApi.GroupsGetById.GetAsync(wallHolder.ID);
         var thisGroup = response.Content.FirstOrDefault();
         if (thisGroup == null) return;

         Name = thisGroup.Name;
         Description = thisGroup.Description;
         ProfilePhoto = thisGroup.Photo200;
      }

      private void onWallContainerPullCompleted(object sender, ObservableCollection<PostViewModel> observableCollection) {
         var wallContainer = sender as WallContainer;
         if (wallContainer == null) return;

         var totalPostCount = wallContainer.GetRealPostCount();
         var repostCount = wallContainer.GetRepostCount();
         var postCount = wallContainer.GetPostOnlyCount();
         var missingPosts = wallContainer.GetMissingPostCount();

         InfoPanel = $"Всего: {totalPostCount} / {WallContainer.MAX_POSTPONED}" +
                     $"\nПостов: {postCount}" +
                     $"\nРепостов: {repostCount}" +
                     $"\nСлоты для отложек: {missingPosts}";
      }
   }
}