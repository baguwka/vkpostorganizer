using System.Threading.Tasks;

namespace vk.Models.Logger {
   public interface IPublishLogger {
      Task LogAsync(HistoryPost data);
      void Log(HistoryPost data);
   }
}