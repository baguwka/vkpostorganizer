using System;
using System.Threading.Tasks;
using System.Windows;
using Data_Persistence_Provider;
using JetBrains.Annotations;

namespace vk {
   [UsedImplicitly]
   public class VkPostponeSaveLoader {
      private readonly SaveLoadController _controller;

      public VkPostponeSaveLoader(SaveLoadController controller) {
         _controller = controller;
      }

      public void Save<T>(string key, T data) where T : class {
         _controller.Save(key, data, onSaveCorrupted);
      }

      [Obsolete("Use LoadInfo<T> TryLoad<T> instead")]
      public bool TryLoad<T>(string key, out T data) where T : class {
         return _controller.TryLoad(key, out data, onLoadCorrupted);
      }

      public LoadInfo<T> TryLoad<T>(string key) where T : class {
         return _controller.TryLoad<T>(key, onLoadCorrupted);
      }

      private bool onLoadCorrupted(DataCorruptedException exception) {
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

      public async Task SaveAsync<T>(string key, T data) where T : class {
         await _controller.SaveAsync(key, data, onSaveCorrupted);
      }

      public async Task<LoadInfo<T>> TryLoadAsync<T>(string key) where T : class {
         return await _controller.TryLoadAsync<T>(key, onLoadCorrupted);
      }

      private bool onSaveCorrupted(DataCorruptedException exception) {
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
