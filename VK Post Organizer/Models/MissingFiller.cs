using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vk.Models.VkApi.Entities;
using vk.Utils;
using vk.ViewModels;

namespace vk.Models {
   public class MissingFiller {
      public const int MAX_POSTPONED = 150;
      public const int RESERVE = 100;

      public bool IsTimeCorrectlyScheduled(int unixTime, ScheduleItem scheduleItem) {
         var dateTime = UnixTimeConverter.ToDateTime(unixTime);
         return new ScheduleItem(dateTime).Equals(scheduleItem);
      }


      public DateTime ConvertScheduleItemToDateTime(DateTime yearMonthDay, ScheduleItem scheduleItem) {
         return new DateTime(yearMonthDay.Year, yearMonthDay.Month, yearMonthDay.Day, scheduleItem.Hour,
            scheduleItem.Minute, 0);
      }

      public async Task<IEnumerable<int>> GetMissingDates(IEnumerable<IPost> posts, Schedule schedule) {
         var tempList = posts.ToList();
         var missing = new List<int>();

         await Task.Run(() => {
            var firstDate = DateTime.Now;
            var nextDate = firstDate;

            int totalCount = tempList.Count();

            while (totalCount < MAX_POSTPONED + RESERVE) {
               var thisDayDate = nextDate;
               var thisDayPosts = tempList.Where(i => UnixTimeConverter.ToDateTime(i.Date).Date == thisDayDate.Date).ToList();

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

                  var isTimeCorrectlyScheduled =
                     thisDayPosts.Any(i => IsTimeCorrectlyScheduled(i.Date, scheduleItem));

                  if (isTimeCorrectlyScheduled == false) {
                     totalCount++;
                     missing.Add(UnixTimeConverter.ToUnix(scheduledDate));
                  }
               }

               nextDate = nextDate.AddDays(1);
            }

            missing.Sort((a, b) => a.CompareTo(b));

            return missing;
         });

         return missing;
      }

      public IEnumerable<VkPostViewModel> BuildMissingViewModels(IEnumerable<int> missingDates) {
         return missingDates.Select(missing => {
            var post = VkPostViewModel.Create(new Post {
               Date = missing,
               ID = 0,
               Message = "Çהוסü למזוע בûעü ןמסע."
            });

            post.Mark = PostMark.Bad;
            post.IsExisting = false;
            post.PostType = PostType.Missing;

            return post;
         });
      }
      
   }
}