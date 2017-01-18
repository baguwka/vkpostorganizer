﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;
using vk.Utils;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallControl : BindableBase {
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

      public ICommand ExpandAllCommand { get; set; }
      public ICommand CollapseAllCommand { get; set; }

      public void Pull() {
         if (WallHolder == null) {
            return;
         }

         Clear();
         Items.AddRange(Pull(WallHolder));
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
         tempList.AddRange(Pull(WallHolder));
         tempList = filter.FilterPosts(tempList).ToList();
         Items.AddRange(tempList);
      }

      public IEnumerable<PostControl> Pull([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         var wall = App.Container.GetInstance<WallGet>();

         try {
            var postList = new List<PostControl>();

            var posts1 = wall.Get(wallHolder.ID);
            postList.AddRange(posts1.Response.Wall.Select(p => new PostControl(p) { IsExisting = true }));

            if (postList.Count == 100) {
               var posts2 = wall.Get(wallHolder.ID, 50, 100);
               postList.AddRange(posts2.Response.Wall.Select(p => new PostControl(p) {IsExisting = true}));
            }

            return postList;
         }
         catch (VkException) {
            Clear();
            throw;
         }
      }

      private void onPostUploadRequested(object sender, EventArgs eventArgs) {
         OnUploadRequested((PostControl)sender);
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

         tempList.AddRange(Pull(WallHolder));

         tempList.Where(i => IsDateMatchTheSchedule(i.Post.DateUnix, schedule)).ForEach(i => i.Mark = PostMark.Good);

         var firstDate = DateTime.Now;
         var nextDate = firstDate;

         int totalCount = tempList.Count;

         while(totalCount < 150) { 
            var thisDayDate = nextDate;
            var thisDayPosts = tempList.Where(i => i.Post.Date.Date == thisDayDate.Date).ToList();

            foreach (var scheduleItem in schedule.Items) {
               if (totalCount >= 150) continue;

               var scheduledDate = ConvertScheduleItemToDateTime(new DateTime(thisDayDate.Year, thisDayDate.Month, thisDayDate.Day), scheduleItem);

               if (scheduledDate <= DateTime.Now) continue;

               var isTimeCorrectlyScheduled = thisDayPosts.Any(i => IsTimeCorrectlyScheduled(i.Post.DateUnix, scheduleItem));

               if (isTimeCorrectlyScheduled == false) {
                  totalCount++;
                  var post = new PostControl(new Post {
                     DateUnix = UnixTimeConverter.ToUnix(scheduledDate),
                     ID = 0,
                     Text = "Здесь должен быть пост."
                  }) {
                     Mark = PostMark.Bad,
                     IsExisting = false,
                     PostType = PostType.Missing
                  };

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