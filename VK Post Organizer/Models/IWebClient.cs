using System;
using System.Threading.Tasks;

namespace vk.Models {
   public interface IWebClient {
      string DownloadString(Uri adress);
      string DownloadString(string adress);

      Task<string> DownloadStringAsync(Uri adress);
      Task<string> DownloadStringAsync(string adress);
   }
}