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
      private SmartCollection<PostItem> _items;

      public WallVM([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         _wallHolder = wallHolder;
         Items = new SmartCollection<PostItem>();

         ExpandAllCommand = new DelegateCommand(expandAllCommandExecute);
         CollapseAllCommand = new DelegateCommand(collapseAllCommandExecute);
      }

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public SmartCollection<PostItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public ICommand ExpandAllCommand { get; set; }
      public ICommand CollapseAllCommand { get; set; }

      public void Pull() {
         if (WallHolder == null) {
            return;
         }

         Pull(WallHolder, NoPostFilter.Instance);
      }

      public void Pull([NotNull] IPostFilter filter) {
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }

         if (WallHolder == null) {
            return;
         }

         Pull(WallHolder, filter);
      }

      public void Pull([NotNull] IWallHolder other, [NotNull] IPostFilter filter, bool clear = true) {
         if (other == null) {
            throw new ArgumentNullException(nameof(other));
         }
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }

         WallHolder = other;

         if (clear) {
            Clear();
         }

         var wall = App.Container.Resolve<WallGet>();

         try {
            var posts1 = wall.Get(WallHolder.ID);
            var posts2 = wall.Get(WallHolder.ID, 50, 100);

            Items.AddRange(filter.FilterPosts(posts1.Response.Wall.Select(p => new PostItem(p))));
            Items.AddRange(filter.FilterPosts(posts2.Response.Wall.Select(p => new PostItem(p))));
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

         if (WallHolder == null) {
            return;
         }

         WallHolder = other;

         Clear();

         Pull(WallHolder, filter, false);

         Items.Where(i => IsDateMatchTheSchedule(i.PostRef.DateUnix, schedule)).ForEach(i => i.Mark = PostMark.Good);

         var firstItem = Items.FirstOrDefault();

         if (firstItem == null) return;

         var firstDate = UnixTimeConverter.ToDateTime(firstItem.PostRef.DateUnix);
         var nextDate = firstDate;
         var totalDays = 150 / schedule.Items.Count;

         for (var day = 1; day < totalDays; day++) {

            foreach (var scheduleItem in schedule.Items) {
               var scheduledDate =
                  ConvertScheduleItemToDateTime(new DateTime(nextDate.Year, nextDate.Month, nextDate.Day), scheduleItem);

               var thisDayPosts = Items.Where(i => i.PostRef.Date.Date == scheduledDate.Date);

               var isTimeCorrectlyScheduled = thisDayPosts.Any(i => IsTimeCorrectlyScheduled(i.PostRef.DateUnix, scheduleItem));

               if (isTimeCorrectlyScheduled == false) {
                  Items.Add(
                     new PostItem(new Post {
                        DateUnix = UnixTimeConverter.ToUnix(scheduledDate),
                        ID = 0,
                        Text = "Здесь должен быть пост."
                     }) {Mark = PostMark.Bad});
               }
            }

            nextDate = nextDate.AddDays(1);
         }
         SortWallByDate();
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

      public void SortWallByDate() {
         var tempList = new List<PostItem>(Items);
         tempList.Sort((a, b) => a.PostRef.DateUnix.CompareTo(b.PostRef.DateUnix));
         Items.Clear();
         Items.AddRange(tempList);
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