using System;
using System.Threading.Tasks;
using System.Windows;
using Data_Persistence_Provider;

namespace vk {
   public static class SaveLoaderHelper {
      public static void Save<T>(string key, T data) where T : class {
         App.Container.GetInstance<SaveLoadController>().Save(key, data, onSaveCorrupted);
      }

      [Obsolete("Use LoadInfo<T> TryLoad<T> instead")]
      public static bool TryLoad<T>(string key, out T data) where T : class {
         return App.Container.GetInstance<SaveLoadController>().TryLoad(key, out data, onLoadCorrupted);
      }

      public static LoadInfo<T> TryLoad<T>(string key) where T : class {
         return App.Container.GetInstance<SaveLoadController>().TryLoad<T>(key, onLoadCorrupted);
      }

      private static bool onLoadCorrupted(DataCorruptedException exception) {
         var result = MessageBox.Show("SaveData corrupted and cannot be loaded. " +
                                      "\nWipe all save data to prevent this error next time? " +
                                      "(Yes is recomended, but if you can restore it somehow manually, then select No)" +
                                      $"\n\n\n Details:\n{exception.Message}" +
                                      $"\n\n StackTrace:\n{exception.StackTrace}",
            "Error", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);

         switch (result) {
            case MessageBoxResult.Yes:
               return true;
            case MessageBoxResult.No:
               return false;
            default:
               return true;
         }
      }

      public static async Task SaveAsync<T>(string key, T data) where T : class {
         await App.Container.GetInstance<SaveLoadController>().SaveAsync(key, data, onSaveCorrupted);
      }

      public static async Task<LoadInfo<T>> TryLoadAsync<T>(string key) where T : class {
         return await App.Container.GetInstance<SaveLoadController>().TryLoadAsync<T>(key, onLoadCorrupted);
      }

      private static bool onSaveCorrupted(DataCorruptedException exception) {
         var result = MessageBox.Show("SaveData corrupted and cannot be saved. " +
                                         "\nBlock writing attempt to not to corrupt the save file? " +
                                         "(Yes is recomended, but if you can restore it somehow manually, then select No)" +
                                         $"\n\n\n Details:\n{exception.Message}" +
                                         $"\n\n StackTrace:\n{exception.StackTrace}",
               "Error", MessageBoxButton.YesNo,
               MessageBoxImage.Error, MessageBoxResult.Yes);

         switch (result) {
            case MessageBoxResult.Yes:
               return true;
            case MessageBoxResult.No:
               return false;
            default:
               return true;
         }
      }
   }
}
