using System;
using System.Threading.Tasks;

namespace vk.Models.Logger {
   public class NoPublishLogger : IPublishLogger {
      public async Task LogAsync(HistoryPost data) {
         await Task.Delay(TimeSpan.Zero);
      }

      public void Log(HistoryPost data) {}
   }
}