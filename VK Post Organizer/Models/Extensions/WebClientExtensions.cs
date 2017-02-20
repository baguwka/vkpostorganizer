using System;
using System.Net;
using System.Threading.Tasks;

namespace vk.Models.Extensions {
   public class DownloadStringTaskAsyncExProgress {
      public int ProgressPercentage { get; set; }
      public string Text { get; set; }
   }

   public static class WebClientExtensions {
      public static async Task<string> DownloadStringTaskAsyncEx(this WebClient wc, string url,
         IProgress<DownloadStringTaskAsyncExProgress> progress) {

         var buffer = new byte[1024];

         var bytes = 0;
         var all = string.Empty;

         using (var stream = await wc.OpenReadTaskAsync(url)) {
            var totalBytes = -1;
            int.TryParse(wc.ResponseHeaders[HttpResponseHeader.ContentLength],
               out totalBytes);

            for (;;) {
               var lenght = await stream.ReadAsync(buffer, 0, buffer.Length);
               if (lenght == 0) break;

               var text = wc.Encoding.GetString(buffer, 0, lenght);

               bytes += lenght;
               all += text;

               if (progress != null) {
                  var args = new DownloadStringTaskAsyncExProgress();
                  args.ProgressPercentage = (totalBytes <= 0 ? 0 : (100 * bytes) / totalBytes);
                  args.Text = text;
                  progress.Report(args); // calls SynchronizationContext.Post
               }
            }
         }

         return all;
      }
   }
}