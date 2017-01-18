using System;
using System.Threading.Tasks;

namespace vk.Models {
   public interface IWebClient {
      string DownloadString(Uri address);
      string DownloadString(string address);

      Task<string> DownloadStringAsync(Uri address);
      Task<string> DownloadStringAsync(string adress);
   }
}