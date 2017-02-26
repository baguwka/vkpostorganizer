using System.Threading.Tasks;

namespace vk.Models.History {
   public interface IHistoryPublisher {
      Task LogAsync(HistoryPost data);
   }
}