namespace vk.Models.Pullers {

   //[UsedImplicitly]
   //public class HistoryPuller : BindableBase {
   //   private readonly JsApiProvider _jsApi;
   //   public const int ITEMS_PER_PAGE = 50;

   //   public HistoryPuller(JsApiProvider jsApi) {
   //      _jsApi = jsApi;
   //      WallHolder = new EmptyWallHolder();
   //      Items = new List<IPost>();
   //   }

   //   public event EventHandler PullInvoked;
   //   public event EventHandler<ContentPullerEventArgs> PullCompleted;
   //   public event EventHandler<IWallHolder> WallHolderChanged;

   //   public DateTimeOffset LastTimePulled { get; private set; }

   //   private IWallHolder _wallHolder;
   //   private List<IPost> _items;

   //   public List<IPost> Items {
   //      get { return _items; }
   //      set { SetProperty(ref _items, value); }
   //   }

   //   public IWallHolder WallHolder {
   //      get { return _wallHolder; }
   //      set {
   //         SetProperty(ref _wallHolder, value);
   //         OnWallHolderChanged(_wallHolder);
   //      }
   //   }

   //   private async Task<IEnumerable<IPost>> pullPostsOfPage(int wallId, int count, int page) {
   //      try {
   //         var response = await _jsApi.GetPosts.GetAsync(wallId, count, page).ConfigureAwait(false);
   //         return response.Content.ToList();
   //      }
   //      catch (JsonServerException) {
   //         //ignore (???) todo: do not ignore
   //      }
   //      return null;
   //   }

   //   public async Task PullAsync() {
   //      if (WallHolder == null) {
   //         throw new ArgumentNullException(nameof(WallHolder));
   //      }

   //      Clear();
   //      OnPullInvoked();
   //      try {
   //         var tempList = new List<IPost>();
   //         tempList.AddRange(await PullAndReturnAsync(WallHolder));
   //         SortListByDate(tempList);
   //         Items.AddRange(tempList);

   //         Debug.WriteLine($"--- HISTORY PULLED AT {DateTimeOffset.Now} IN {Thread.CurrentContext.ContextID} THREAD");
   //         OnPulled(new ContentPullerEventArgs {Successful = true, Items = Items});
   //      }
   //      catch {
   //         Clear();
   //         OnPulled(new ContentPullerEventArgs {Successful = false});
   //         throw;
   //      }
   //   }

   //   public static void SortListByDate(List<IPost> posts) {
   //      posts.Sort((a, b) => b.Date.CompareTo(a.Date));
   //   }

   //   public async Task<IEnumerable<IPost>> PullAndReturnAsync(IWallHolder wallHolder) {
   //      if (wallHolder == null) {
   //         throw new ArgumentNullException(nameof(wallHolder));
   //      }

   //      WallHolder = wallHolder;

   //      try {
   //         var postList = new List<IPost>();
   //         postList.AddRange(await pullPostsOfPage(WallHolder.ID, ITEMS_PER_PAGE, 0).ConfigureAwait(false));
   //         return postList;
   //      }
   //      catch (JsonServerException) {
   //         throw;
   //      }
   //   }

   //   public void Clear() {
   //      //foreach (var postItem in Items) {
   //      //   postItem.ClearPreview();
   //      //}
   //      Items.Clear();
   //   }

   //   //public void ExpandAll() {
   //   //   foreach (var postItem in Items) {
   //   //      postItem.Expand();
   //   //   }
   //   //}

   //   //public void CollapseAll() {
   //   //   foreach (var postItem in Items) {
   //   //      postItem.Collapse();
   //   //   }
   //   //}

   //   protected virtual void OnPulled(ContentPullerEventArgs e) {
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