using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using vk.Models.VkApi.Entities;

namespace vk.Models.Pullers {
   public sealed class ContentPuller : IContentPuller {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

      private readonly IContentPullerStrategy _contentPullerStrategy;
      private IWallHolder _wallHolder;

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set {
            _wallHolder = value; 
            OnWallHolderChanged(_wallHolder);
         }
      }

      public DateTimeOffset LastTimePulled { get; private set; }

      public List<IPost> Items { get; }
      public event EventHandler PullInvoked;
      public event EventHandler<ContentPullerEventArgs> PullCompleted;
      public event EventHandler<IWallHolder> WallHolderChanged;

      public ContentPuller(IContentPullerStrategy contentPullerStrategy) {
         _contentPullerStrategy = contentPullerStrategy;
         Items = new List<IPost>();
         WallHolder = new WallHolder(0);
      }

      private readonly object _locker = new object();

      public async Task PullAsync(PullerSettings settings, CancellationToken ct) {
         // ReSharper disable once InconsistentlySynchronizedField
         logger.Debug($"Попытка сделать pull контента из источника");
         OnPullInvoked();
         try {
            Items.Clear();
            var posts = await _contentPullerStrategy.GetAsync(WallHolder, settings, ct);
            lock (_locker) {
               Items.AddRange(posts);
               LastTimePulled = DateTimeOffset.Now;
               logger.Debug($"Pull удачный. Получено элементов - {Items?.Count}");
               OnPullCompleted(new ContentPullerEventArgs { Successful = true, Items = Items });
            }
         }
         catch (Exception ex) {
            // ReSharper disable once InconsistentlySynchronizedField
            logger.Debug(ex, "Ошибка при пуллинге контента. Очистка текущих элементов.");
            Items.Clear();
            OnPullCompleted(new ContentPullerEventArgs {Successful = false});
         }
      }

      public async Task PullAsync(PullerSettings settings) {
         await PullAsync(settings, CancellationToken.None);
      }

      public async Task PullAsync(CancellationToken ct) {
         await PullAsync(PullerSettings.No, ct);
      }

      public async Task PullAsync() {
         await PullAsync(PullerSettings.No, CancellationToken.None);
      }

      private void OnPullInvoked() {
         PullInvoked?.Invoke(this, EventArgs.Empty);
      }

      private void OnPullCompleted(ContentPullerEventArgs e) {
         PullCompleted?.Invoke(this, e);
      }

      private void OnWallHolderChanged(IWallHolder e) {
         logger.Trace($"WallHolder был изменен. Новое Id - {e.ID}, Name - {e.Name}");
         WallHolderChanged?.Invoke(this, e);
      }
   }
}