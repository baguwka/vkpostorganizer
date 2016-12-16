using System.Windows;
using Data_Persistence_Provider;
using Microsoft.Practices.Unity;

namespace vk {
   public static class SaveLoaderHelper {
      public static void Save<T>(string key, T data) where T : CommonSaveData {
         App.Container.Resolve<SaveLoadController>().Save(key, data, onSaveCorrupted);
      }

      public static bool TryLoad<T>(string key, out T data) where T : CommonSaveData {
         return App.Container.Resolve<SaveLoadController>().TryLoad(key, out data, onLoadCorrupted);
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

      private static bool onSaveCorrupted(DataCorruptedException exception) {
         var result = MessageBox.Show("SaveData corrupted and cannot be saved. " +
                                         "\nBlock writing attempt to not to corrupt the save file?? " +
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
