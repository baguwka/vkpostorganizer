using System;

namespace vk.Models {
   public interface IWebClient {
      string DownloadString(Uri address);
      string DownloadString(string address);
   }
}