using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public interface IContentPuller {
      IWallHolder WallHolder { get; set; }
      List<IPost> Items { get; }
      DateTimeOffset LastTimePulled { get; }

      event EventHandler PullInvoked;
      event EventHandler<ContentPullerEventArgs> PullCompleted;
      event EventHandler<IWallHolder> WallHolderChanged;

      Task PullAsync();
      Task PullAsync(CancellationToken ct);
   }
}