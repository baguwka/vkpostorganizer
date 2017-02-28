using System.Collections.Generic;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public class ContentPullerEventArgs {
      public IList<IPost> Items { get; set; }
      public bool Successful { get; set; }
   }


   //[UsedImplicitly]
   //public class VkWallPuller : BindableBase {
   //   private readonly VkApiProvider _vkApi;
   //   public event EventHandler PullInvoked;
   //   public event EventHandler<VkPullerEventArgs> PullCompleted;
   //   public event EventHandler<IWallHolder> WallHolderChanged;

   //   private IWallHolder _wallHolder;
   //   private List<IPost> _items;

   //   public DateTimeOffset LastTimePulled { get; private set; }

   //   public VkWallPuller(VkApiProvider vkApi) {
   //      _vkApi = vkApi;
   //      WallHolder = new EmptyWallHolder();

   //      Items = new List<IPost>();
   //   }

   //   public event EventHandler<IPost> UploadRequested;

   //   public IWallHolder WallHolder {
   //      get { return _wallHolder; }
   //      set {
   //         SetProperty(ref _wallHolder, value);
   //         OnWallHolderChanged(_wallHolder);
   //      }
   //   }

   //   public List<IPost> Items {
   //      get { return _items; }
   //      set { SetProperty(ref _items, value); }
   //   }

   //   private async Task<IEnumerable<IPost>> pullPostsWithAnOffset(int wallHolderId, int count, int offset) {
   //      //var response = await _vkApi.WallGet.GetAsync(wallHolderId, count, offset, true).ConfigureAwait(false);
   //      //return response.Content.Wall.ToList();
   //      //var tasks = response.Content.Wall.Select(async p => {
   //      //   var post = await IPost.CreateAsync(p).ConfigureAwait(false);
   //      //   post.IsExisting = true;
   //      //   return post;
   //      //}).ToList();

   //      //return await Task.WhenAll(tasks).ConfigureAwait(false);
   //   }

   //   private async Task<IEnumerable<IPost>> pullAsync(IWallHolder wallHolder) {
   //      if (wallHolder == null) {
   //         throw new ArgumentNullException(nameof(wallHolder));
   //      }

   //      try {
   //         var postList = new List<IPost>();

   //         postList.AddRange(await pullPostsWithAnOffset(wallHolder.ID, 100, 0).ConfigureAwait(false));
   //         if (postList.Count == 100) {
   //            postList.AddRange(await pullPostsWithAnOffset(wallHolder.ID, 50, 100).ConfigureAwait(false));
   //         }

   //         return postList;
   //      }
   //      catch (VkException) {
   //         Clear();
   //         throw;
   //      }
   //   } 

   //   private void onPostUploadRequested(object sender, EventArgs eventArgs) {
   //      OnUploadRequested((IPost)sender);
   //   }

   //   public async Task PullAsync() {
   //      if (WallHolder == null) {
   //         return;
   //      }

   //      Clear();
   //      OnPullInvoked();
   //      try {
   //         var tempList = new List<IPost>();
   //         tempList.AddRange(await pullAsync(WallHolder).ConfigureAwait(false));
   //         SortListByDate(tempList);
   //         Items.AddRange(tempList);
   //         OnPulled(new VkPullerEventArgs {Successful = true, Items = Items});
   //      }
   //      catch {
   //         Clear();
   //         OnPulled(new VkPullerEventArgs {Successful = false});
   //         throw;
   //      }
   //   }

   //   //public int GetRealPostCount() {
   //   //   return Items.Count(post => post.IsExisting);
   //   //}

   //   //public int GetRepostCount() {
   //   //   return Items.Count(post => post.IsExisting && post.PostType == PostType.Repost);
   //   //}

   //   //public int GetPostOnlyCount() {
   //   //   return Items.Count(post => post.IsExisting && post.PostType == PostType.Post);
   //   //}

   //   //public int GetMissingPostCountWithReserve() {
   //   //   return Items.Count(post => post.PostType == PostType.Missing);
   //   //}

   //   //public int GetMissingPostCount() {
   //   //   return GetMissingPostCountWithReserve() - RESERVE;
   //   //}

   //   public static void SortListByDate(List<IPost> posts) {
   //      posts.Sort((a, b) => a.Date.CompareTo(b.Date));
   //   }

   //   public void Clear() {
   //      //foreach (var postItem in Items) {
   //      //   postItem.ClearPreview();
   //      //   postItem.UploadRequested -= onPostUploadRequested;
   //      //}
   //      Items.Clear();
   //   }

      //public void ExpandAll() {
      //   foreach (var postItem in Items) {
      //      postItem.Expand();
      //   }
      //}

      //public void CollapseAll() {
      //   foreach (var postItem in Items) {
      //      postItem.Collapse();
      //   }
      //}

   //   protected virtual void OnUploadRequested(IPost e) {
   //      UploadRequested?.Invoke(this, e);
   //   }

   //   protected virtual void OnPulled(VkPullerEventArgs e) {
   //      LastTimePulled = DateTimeOffset.Now;
   //      PullCompleted?.Invoke(this, e);
   //   }

   //   protected virtual void OnPullInvoked() {
   //      PullInvoked?.Invoke(this, EventArgs.Empty);
   //   }

   //   protected virtual void OnWallHolderChanged(IWallHolder e) {
   //      WallHolderChanged?.Invoke(this, e);
   //   }
   //}
}