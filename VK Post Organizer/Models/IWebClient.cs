using System;
using System.Threading.Tasks;

namespace vk.Models {
   public interface IWebClient {
      Task<string> DownloadStringAsync(Uri address);
      Task<string> DownloadStringAsync(string adress);
   }
}