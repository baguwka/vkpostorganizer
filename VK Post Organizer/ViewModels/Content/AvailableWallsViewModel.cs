using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using Prism.Events;
using Prism.Mvvm;
using vk.Events;
using vk.Models;
using vk.Models.VkApi;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class AvailableWallsViewModel : BindableBase {
      private readonly IEventAggregator _eventAggregator;
      private WallList _wallList;

      public WallList WallList {
         get { return _wallList; }
         set { SetProperty(ref _wallList, value); }
      }

      private bool _isAuthorized;

      public bool IsAuthorized {
         get { return _isAuthorized; }
         set { SetProperty(ref _isAuthorized, value); }
      }

      public AvailableWallsViewModel(IEventAggregator eventAggregator, WallList wallList) {
         _eventAggregator = eventAggregator;
         WallList = wallList;
         WallList.ItemClicked += onWallItemClicked;
         _eventAggregator.GetEvent<WallSelectorEvents.FillWallRequest>().Subscribe(fillWallList);
         _eventAggregator.GetEvent<AuthBarEvents.AuthorizationCompleted>()
            .Subscribe(authorized => IsAuthorized = authorized);
         _eventAggregator.GetEvent<AuthBarEvents.LogOutCompleted>()
            .Subscribe(() => IsAuthorized = false);
      }

      private void onWallItemClicked(object sender, WallItem e) {
         if (!IsAuthorized) return;

         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Publish(e);
      }

      private async void fillWallList() {
         var methodGroupsGet = App.Container.GetInstance<GroupsGet>();
         var groups = await methodGroupsGet.GetAsync();
         if (groups.Collection == null) {
            MessageBox.Show("Groups not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }

         var userget = App.Container.GetInstance<UsersGet>();
         var users = await userget.GetAsync();
         var user = users.Users.FirstOrDefault();
         if (user == null) {
            MessageBox.Show("User not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }

         //todo: get rid of workaround
         var item = new WallItem(new EmptyWallHolder {
            Name = user.Name,
            Description = user.Description,
            Photo200 = user.Photo200,
            Photo50 = user.Photo50
         });

         _wallList.Clear();
         _wallList.Add(item);

         foreach (var group in groups.Collection.Groups) {
            _wallList.Add(new WallItem(group));
         }
      }
   }
}