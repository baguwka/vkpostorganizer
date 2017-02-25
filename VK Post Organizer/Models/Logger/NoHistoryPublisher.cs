using System;
using System.Threading.Tasks;

namespace vk.Models.Logger {
   public class NoHistoryPublisher : IHistoryPublisher {
      public async Task LogAsync(HistoryPost data) {
         await Task.Delay(TimeSpan.Zero).ConfigureAwait(false);
      }

      public void Log(HistoryPost data) {}
   }
}