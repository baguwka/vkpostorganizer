using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Mvvm;
using vk.Models;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallControl : BindableBase {
      public const int MAX_POSTPONED = 150;
      public const int RESERVE = 100;

      private IWallHolder _wallHolder;
      private SmartCollection<PostControl> _items;

      public event EventHandler<PostControl> UploadRequested;

      public WallControl([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         _wallHolder = wallHolder;
         Items = new SmartCollection<PostControl>();

         ExpandAllCommand = new DelegateCommand(expandAllCommandExecute);
         CollapseAllCommand = new DelegateCommand(collapseAllCommandExecute);
      }

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public SmartCollection<PostControl> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public ICommand ExpandAllCommand { get; private set; }
      public ICommand CollapseAllCommand { get; private set; }

      //public void Pull() {
      //   if (WallHolder == null) {
      //      return;
      //   }

      //   Clear();
      //   Items.AddRange(Pull(WallHolder));
      //}

      //public void Pull([NotNull] PostFilter filter) {
      //   if (filter == null) {
      //      throw new ArgumentNullException(nameof(filter));
      //   }

      //   if (WallHolder == null) {
      //      return;
      //   }

      //   Clear();
      //   var tempList = new List<PostControl>();
      //   tempList.AddRange(Pull(WallHolder));
      //   tempList = filter.FilterPosts(tempList).ToList();
      //   Items.AddRange(tempList);
      //}

      private static async Task<IEnumerable<PostControl>> pullPostsWithAnOffset(WallGet wall, int wallHolderId, int count, int offset) {
         var posts = await wall.GetAsync(wallHolderId, count, offset);
         var tasks = posts.Response.Wall.Select(async p => {
            var post = await PostControl.CreateAsync(p);
            post.IsExisting = true;
            return post;
         }).ToList();

         return await Task.WhenAll(tasks);
      }

      public async Task<IEnumerable<PostControl>> PullAsync(IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         var wall = App.Container.GetInstance<WallGet>();

         try {
            var postList = new List<PostControl>();

            postList.AddRange(await pullPostsWithAnOffset(wall, wallHolder.ID, 100, 0));
            if (postList.Count == 100) {
               postList.AddRange(await pullPostsWithAnOffset(wall, wallHolder.ID, 50, 100));
            }

            return postList;
         }
         catch (VkException) {
            Clear();
            throw;
         }
      } 

      //public IEnumerable<PostControl> Pull([NotNull] IWallHolder wallHolder) {
      //   if (wallHolder == null) {
      //      throw new ArgumentNullException(nameof(wallHolder));
      //   }

      //   var wall = App.Container.GetInstance<WallGet>();

      //   try {
      //      var postList = new List<PostControl>();

      //      var posts1 = wall.Get(wallHolder.ID);
      //      postList.AddRange(posts1.Response.Wall.Select(p => new PostControl(p) { IsExisting = true }));

      //      if (postList.Count == 100) {
      //         var posts2 = wall.Get(wallHolder.ID, 50, 100);
      //         postList.AddRange(posts2.Response.Wall.Select(p => new PostControl(p) {IsExisting = true}));
      //      }

      //      return postList;
      //   }
      //   catch (VkException) {
      //      Clear();
      //      throw;
      //   }
      //}

      private void onPostUploadRequested(object sender, EventArgs eventArgs) {
         OnUploadRequested((PostControl)sender);
      }

      //public void PullWithScheduleHightlight([NotNull] PostFilter filter, [NotNull] Schedule schedule) {
      //   if (filter == null) {
      //      throw new ArgumentNullException(nameof(filter));
      //   }
      //   if (schedule == null) {
      //      throw new ArgumentNullException(nameof(schedule));
      //   }
      //   if (WallHolder == null) {
      //      return;
      //   }

      //   PullWithScheduleHightlight(WallHolder, filter, schedule);
      //}

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

         await PullWithScheduleHightlightAsync(WallHolder, filter, schedule);
      }


      //public void PullWithScheduleHightlight([NotNull] IWallHolder other, [NotNull] PostFilter filter, [NotNull] Schedule schedule) {
      //   if (other == null) {
      //      throw new ArgumentNullException(nameof(other));
      //   }
      //   if (filter == null) {
      //      throw new ArgumentNullException(nameof(filter));
      //   }
      //   if (schedule == null) {
      //      throw new ArgumentNullException(nameof(schedule));
      //   }

      //   WallHolder = other;

      //   Clear();

      //   var tempList = new List<PostControl>();

      //   tempList.AddRange(Pull(WallHolder));

      //   addMissingFakePosts(filter, schedule, tempList);
      //}

      public async Task PullWithScheduleHightlightAsync([NotNull] IWallHolder other, [NotNull] PostFilter filter, [NotNull] Schedule schedule) {
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

         var tempList = new List<PostControl>();

         tempList.AddRange(await PullAsync(WallHolder));

         await addMissingFakePosts(filter, schedule, tempList);
      }

      private async Task addMissingFakePosts(PostFilter filter, Schedule schedule, List<PostControl> tempList) {
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

                  var post = await PostControl.CreateAsync(new Post {
                     DateUnix = UnixTimeConverter.ToUnix(scheduledDate),
                     ID = 0,
                     Text = "Здесь должен быть пост."
                  });
                  
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

         tempList = filter.FilterPosts(tempList).ToList();
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

      public static void SortListByDate(List<PostControl> posts) {
         posts.Sort((a, b) => a.Post.DateUnix.CompareTo(b.Post.DateUnix));
      }

      public void Clear() {
         foreach (var postItem in Items) {
            postItem.Clear();
            postItem.UploadRequested -= onPostUploadRequested;
         }
         Items.Clear();
      }

      private void expandAllCommandExecute() {
         foreach (var postItem in Items) {
            postItem.Expand();
         }
      }

      private void collapseAllCommandExecute() {
         foreach (var postItem in Items) {
            postItem.Collapse();
         }
      }

      protected virtual void OnUploadRequested(PostControl e) {
         UploadRequested?.Invoke(this, e);
      }
   }
}