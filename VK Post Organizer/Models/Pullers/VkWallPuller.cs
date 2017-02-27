using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Prism.Mvvm;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;
using vk.ViewModels;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class VkWallPuller : BindableBase {
      private readonly VkApiProvider _vkApi;
      public const int MAX_POSTPONED = 150;
      public const int RESERVE = 100;
      public event EventHandler PullInvoked;
      public event EventHandler<IList<PostViewModel>> PullCompleted;
      public event EventHandler<IWallHolder> WallHolderChanged;

      private IWallHolder _wallHolder;
      private ObservableCollection<PostViewModel> _items;

      public VkWallPuller(VkApiProvider vkApi) {
         _vkApi = vkApi;
         WallHolder = new EmptyWallHolder();

         Items = new ObservableCollection<PostViewModel>();
      }

      public event EventHandler<PostViewModel> UploadRequested;

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set {
            SetProperty(ref _wallHolder, value);
            OnWallHolderChanged(_wallHolder);
         }
      }

      public ObservableCollection<PostViewModel> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      private async Task<IEnumerable<PostViewModel>> pullPostsWithAnOffset(int wallHolderId, int count, int offset) {
         var response = await _vkApi.WallGet.GetAsync(wallHolderId, count, offset, true).ConfigureAwait(false);
         var tasks = response.Content.Wall.Select(async p => {
            var post = await PostViewModel.CreateAsync(p).ConfigureAwait(false);
            post.IsExisting = true;
            return post;
         }).ToList();

         return await Task.WhenAll(tasks).ConfigureAwait(false);
      }

      public async Task<IEnumerable<PostViewModel>> PullAsync(IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         try {
            var postList = new List<PostViewModel>();

            postList.AddRange(await pullPostsWithAnOffset(wallHolder.ID, 100, 0).ConfigureAwait(false));
            if (postList.Count == 100) {
               postList.AddRange(await pullPostsWithAnOffset(wallHolder.ID, 50, 100).ConfigureAwait(false));
            }

            return postList;
         }
         catch (VkException) {
            Clear();
            throw;
         }
      } 

      private void onPostUploadRequested(object sender, EventArgs eventArgs) {
         OnUploadRequested((PostViewModel)sender);
      }

      public async Task PullWithScheduleHightlightAsync([NotNull] PostFilter filter, [NotNull] Schedule schedule) {
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }
         if (schedule == null) {
            throw new ArgumentNullException(nameof(schedule));
         }
         if (WallHolder == null) {
            return;
         }

         OnPullInvoked();
         try {
            await PullWithScheduleHightlightAsync(WallHolder, filter, schedule);
         }
         finally {
            OnPulled(Items);
         }
      }

      private async Task PullWithScheduleHightlightAsync([NotNull] IWallHolder other, [NotNull] PostFilter filter, [NotNull] Schedule schedule) {
         if (other == null) {
            throw new ArgumentNullException(nameof(other));
         }
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }
         if (schedule == null) {
            throw new ArgumentNullException(nameof(schedule));
         }

         WallHolder = other;

         Clear();

         var tempList = new List<PostViewModel>();

         tempList.AddRange(await PullAsync(WallHolder).ConfigureAwait(false));

         await addMissingFakePosts(filter, schedule, tempList).ConfigureAwait(false);
      }

      private async Task addMissingFakePosts(PostFilter filter, Schedule schedule, List<PostViewModel> tempList) {
         tempList.Where(i => IsDateMatchTheSchedule(i.Post.DateUnix, schedule)).ForEach(i => i.Mark = PostMark.Good);

         var firstDate = DateTime.Now;
         var nextDate = firstDate;

         int totalCount = tempList.Count;

         while (totalCount < MAX_POSTPONED + RESERVE) {
            var thisDayDate = nextDate;
            var thisDayPosts = tempList.Where(i => i.Post.Date.Date == thisDayDate.Date).ToList();

            foreach (var scheduleItem in schedule.Items) {
               if (totalCount >= MAX_POSTPONED + RESERVE) {
                  break;
               }

               var scheduledDate =
                  ConvertScheduleItemToDateTime(new DateTime(thisDayDate.Year, thisDayDate.Month, thisDayDate.Day),
                     scheduleItem);

               if (scheduledDate <= DateTime.Now) {
                  continue;
               }

               var isTimeCorrectlyScheduled = thisDayPosts.Any(i => IsTimeCorrectlyScheduled(i.Post.DateUnix, scheduleItem));

               if (isTimeCorrectlyScheduled == false) {
                  totalCount++;

                  var post = await PostViewModel.CreateAsync(new Post {
                     DateUnix = UnixTimeConverter.ToUnix(scheduledDate),
                     ID = 0,
                     Text = "Здесь должен быть пост."
                  }).ConfigureAwait(false);
                  
                  post.Mark = PostMark.Bad;
                  post.IsExisting = false;
                  post.PostType = PostType.Missing;

                  //var post = new PostControl(new Post {
                  //   DateUnix = UnixTimeConverter.ToUnix(scheduledDate),
                  //   ID = 0,
                  //   Text = "Здесь должен быть пост."
                  //}) {
                  //   Mark = PostMark.Bad,
                  //   IsExisting = false,
                  //   PostType = PostType.Missing
                  //};

                  post.UploadRequested += onPostUploadRequested;
                  tempList.Add(post);
               }
            }

            nextDate = nextDate.AddDays(1);
         }

         tempList = tempList.Where(filter.Suitable).ToList();
         SortListByDate(tempList);

         Items.AddRange(tempList);
      }

      public int GetRealPostCount() {
         return Items.Count(post => post.IsExisting);
      }

      public int GetRepostCount() {
         return Items.Count(post => post.IsExisting && post.PostType == PostType.Repost);
      }

      public int GetPostOnlyCount() {
         return Items.Count(post => post.IsExisting && post.PostType == PostType.Post);
      }

      public int GetMissingPostCountWithReserve() {
         return Items.Count(post => post.PostType == PostType.Missing);
      }

      public int GetMissingPostCount() {
         return GetMissingPostCountWithReserve() - RESERVE;
      }

      public bool IsTimeCorrectlyScheduled(int unixTime, ScheduleItem scheduleItem) {
         var dateTime = UnixTimeConverter.ToDateTime(unixTime);
         return new ScheduleItem(dateTime).Equals(scheduleItem);
      }

      public bool IsDateMatchTheSchedule(int unixTime, Schedule schedule) {
         var dateTime = UnixTimeConverter.ToDateTime(unixTime);
         return schedule.Items.Any(i => i.Hour == dateTime.Hour && i.Minute == dateTime.Minute);
      }

      public DateTime ConvertScheduleItemToDateTime(DateTime yearMonthDay, ScheduleItem scheduleItem) {
         return new DateTime(yearMonthDay.Year, yearMonthDay.Month, yearMonthDay.Day, scheduleItem.Hour, scheduleItem.Minute, 0);
      }

      public static void SortListByDate(List<PostViewModel> posts) {
         posts.Sort((a, b) => a.Post.DateUnix.CompareTo(b.Post.DateUnix));
      }

      public void Clear() {
         foreach (var postItem in Items) {
            postItem.ClearPreview();
            postItem.UploadRequested -= onPostUploadRequested;
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

      protected virtual void OnUploadRequested(PostViewModel e) {
         UploadRequested?.Invoke(this, e);
      }

      protected virtual void OnPulled(IList<PostViewModel> collection) {
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