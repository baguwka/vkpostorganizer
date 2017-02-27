using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Mvvm;
using vk.Models.JsonServerApi;
using vk.ViewModels;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class HistoryPuller : BindableBase {
      private readonly JsApiProvider _jsApi;
      public const int ITEMS_PER_PAGE = 50;

      public HistoryPuller(JsApiProvider jsApi) {
         _jsApi = jsApi;
         WallHolder = new EmptyWallHolder();
         Items = new ObservableCollection<HistoryPostViewModel>();
      }

      public event EventHandler PullInvoked;
      public event EventHandler<IList<HistoryPostViewModel>> PullCompleted;
      public event EventHandler<IWallHolder> WallHolderChanged;

      private IWallHolder _wallHolder;
      private ObservableCollection<HistoryPostViewModel> _items;

      public ObservableCollection<HistoryPostViewModel> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set {
            SetProperty(ref _wallHolder, value);
            OnWallHolderChanged(_wallHolder);
         }
      }

      private async Task<IEnumerable<HistoryPostViewModel>> pullPostsOfPage(int wallId, int count, int page) {
         try {
            var response = await _jsApi.GetPosts.GetAsync(wallId, count, page).ConfigureAwait(false);
            return response.Content.Select(HistoryPostViewModel.Create).ToList();
         }
         catch (JsonServerException) {
            //ignore (???) todo: do not ignore
         }
         return null;
      }

      public async Task PullAsync() {
         if (WallHolder == null) {
            throw new ArgumentNullException(nameof(WallHolder));
         }

         OnPullInvoked();
         try {
            var tempList = new List<HistoryPostViewModel>();
            tempList.AddRange(await PullAndReturnAsync(WallHolder));
            SortListByDate(tempList);
            Items.AddRange(tempList);
         }
         finally {
            OnPulled(Items);
         }
      }

      public static void SortListByDate(List<HistoryPostViewModel> posts) {
         posts.Sort((a, b) => b.Post.PublishingDateUnix.CompareTo(a.Post.PublishingDateUnix));
      }

      public async Task<IEnumerable<HistoryPostViewModel>> PullAndReturnAsync(IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         WallHolder = wallHolder;
         Clear();

         try {
            var postList = new List<HistoryPostViewModel>();
            postList.AddRange(await pullPostsOfPage(WallHolder.ID, ITEMS_PER_PAGE, 0).ConfigureAwait(false));
            return postList;
         }
         catch (JsonServerException) {
            Clear();
            throw;
         }
      }

      public void Clear() {
         foreach (var postItem in Items) {
            postItem.ClearPreview();
         }
         Items.Clear();
      }

      public void ExpandAll() {
         foreach (var postItem in Items) {
            postItem.Expand();
         }
      }

      public void CollapseAll() {
         foreach (var postItem in Items) {
            postItem.Collapse();
         }
      }

      protected virtual void OnPulled(IList<HistoryPostViewModel> collection) {
         PullCompleted?.Invoke(this, collection);
      }

      protected virtual void OnPullInvoked() {
         PullInvoked?.Invoke(this, EventArgs.Empty);
      }

      protected virtual void OnWallHolderChanged(IWallHolder e) {
         WallHolderChanged?.Invoke(this, e);
      }
   }
}