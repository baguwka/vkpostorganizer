using System;

namespace vk.Models {
   public interface IWebClient {
      string DownloadString(Uri address);
      string DownloadString(string address);

      byte[] UploadFile(string url, string method, string path);
   }
}