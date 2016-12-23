using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallVM : BindableBase {
      private IWallHolder _wallHolder;
      private SmartCollection<PostControl> _items;

      public WallVM([NotNull] IWallHolder wallHolder) {
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

      public ICommand ExpandAllCommand { get; set; }
      public ICommand CollapseAllCommand { get; set; }

      public void Pull() {
         if (WallHolder == null) {
            return;
         }

         Clear();
         Items.AddRange(pull(WallHolder));
      }

      public void Pull([NotNull] IPostFilter filter) {
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }

         if (WallHolder == null) {
            return;
         }

         Clear();
         var tempList = new List<PostControl>();
         tempList.AddRange(pull(WallHolder));
         tempList = filter.FilterPosts(tempList).ToList();
         Items.AddRange(tempList);
      }

      private IEnumerable<PostControl> pull([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         var wall = App.Container.Resolve<WallGet>();

         try {
            var postList = new List<PostControl>();

            var posts1 = wall.Get(wallHolder.ID);
            var posts2 = wall.Get(wallHolder.ID, 50, 100);

            postList.AddRange(posts1.Response.Wall.Select(p => new PostControl(p) { IsExisting = true }));
            postList.AddRange(posts2.Response.Wall.Select(p => new PostControl(p) { IsExisting = true }));

            return postList;
         }
         catch (VkException) {
            Clear();
            throw;
         }
      }

      public void PullWithScheduleHightlight([NotNull] IPostFilter filter, [NotNull] Schedule schedule) {
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }
         if (schedule == null) {
            throw new ArgumentNullException(nameof(schedule));
         }
         if (WallHolder == null) {
            return;
         }

         PullWithScheduleHightlight(WallHolder, filter, schedule);
      }


      public void PullWithScheduleHightlight([NotNull] IWallHolder other, [NotNull] IPostFilter filter, [NotNull] Schedule schedule) {
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

         tempList.AddRange(pull(WallHolder));

         tempList.Where(i => IsDateMatchTheSchedule(i.Post.DateUnix, schedule)).ForEach(i => i.Mark = PostMark.Good);

         var firstItem = tempList.FirstOrDefault();

         if (firstItem == null) return;

         var firstDate = DateTime.Now;
         var nextDate = firstDate;

         int totalCount = 0;

         //for (var day = 1; day <= totalDays + 10; day++) {
         while(totalCount <= 150) { 
            var thisDayDate = nextDate;
            var thisDayPosts = tempList.Where(i => i.Post.Date.Date == thisDayDate.Date).ToList();

            foreach (var scheduleItem in schedule.Items) {
               if (totalCount > 150) continue;
               totalCount++;

               var scheduledDate = ConvertScheduleItemToDateTime(new DateTime(thisDayDate.Year, thisDayDate.Month, thisDayDate.Day), scheduleItem);


               if (scheduledDate <= DateTime.Now) continue;

               var isTimeCorrectlyScheduled = thisDayPosts.Any(i => IsTimeCorrectlyScheduled(i.Post.DateUnix, scheduleItem));

               if (isTimeCorrectlyScheduled == false) {
                  tempList.Add(
                     new PostControl(new Post {
                        DateUnix = UnixTimeConverter.ToUnix(scheduledDate),
                        ID = 0,
                        Text = "Здесь должен быть пост."
                     }) {
                        Mark = PostMark.Bad,
                        IsExisting = false,
                        PostType = PostType.Missing
                     });
               }
            }

            nextDate = nextDate.AddDays(1);
         }

         tempList = filter.FilterPosts(tempList).ToList();
         SortListByDate(tempList);

         Items.AddRange(tempList);
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
   }
}