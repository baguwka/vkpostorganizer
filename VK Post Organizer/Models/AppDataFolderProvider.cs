using System;
using System.IO;
using System.Threading.Tasks;
using Data_Persistence_Provider;
using JetBrains.Annotations;

namespace vk.Models {
   [UsedImplicitly]
   public class AppDataFolderProvider : IDataProvider {
      private string _dataPath;

      private string dataPath {
         get {
            if (string.IsNullOrEmpty(_dataPath)) {
               _dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                           "\\Baguwk\\Vk Postpone Helper\\";
               if (!Directory.Exists(_dataPath)) {
                  Directory.CreateDirectory(_dataPath);
               }
            }
            return _dataPath;
         }
      }

      private string getFilePath(string key) {
         return $"{dataPath}/{fileMask(key)}.json";
      }

      private static string fileMask(string filename) {
         return $"{filename}.bgw";
      }

      public string Read(string key) {
         var filepath = getFilePath(key);

         if (!File.Exists(filepath)) {
            return null;
         }

         using (var content = File.OpenText(filepath)) {
            return content.ReadToEnd();
         }
      }

      public void Write(string key, string data) {
         using (var stream = new StreamWriter(getFilePath(key))) {
            stream.Write(data);
         }
      }

      public async Task<string> ReadAsync(string key) {
         var filepath = getFilePath(key);

         if (!File.Exists(filepath)) {
            return null;
         }

         using (var content = File.OpenText(filepath)) {
            return await content.ReadToEndAsync();
         }
      }

      public async Task WriteAsync(string key, string data) {
         using (var stream = new StreamWriter(getFilePath(key))) {
            await stream.WriteAsync(data);
         }
      }
   }
}