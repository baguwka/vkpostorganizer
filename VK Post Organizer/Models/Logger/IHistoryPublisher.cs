using System.Threading.Tasks;

namespace vk.Models.Logger {
   public interface IHistoryPublisher {
      Task LogAsync(HistoryPost data);
   }
}