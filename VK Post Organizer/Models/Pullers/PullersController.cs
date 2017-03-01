using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using JetBrains.Annotations;
using Prism.Events;
using vk.Events;
using Timer = System.Timers.Timer;

namespace vk.Models.Pullers {
   [UsedImplicitly]
   public class PullersController {
      private readonly IEventAggregator _eventAggregator;
      private readonly Settings _settings;
      private IWallHolder _sharedWallHolder;
      private Timer _historyPullTimer;
      public IContentPuller Actual { get; }
      public IContentPuller Postponed { get; }
      public IContentPuller History { get; }

      public IWallHolder SharedWallHolder {
         get { return _sharedWallHolder; }
         set {
            _sharedWallHolder = value;
            Actual.WallHolder = _sharedWallHolder;
            Postponed.WallHolder = _sharedWallHolder;
            History.WallHolder = _sharedWallHolder;
         }
      }

      private readonly Dispatcher _currentDispatcher;

      public PullersController(IEventAggregator eventAggregator, PullerStrategies strategies, Settings settings) {
         _eventAggregator = eventAggregator;
         _settings = settings;
         _currentDispatcher = Dispatcher.CurrentDispatcher;

         Actual = new ContentPuller(strategies.ActualPullerStrategyStrategy);
         Postponed = new ContentPuller(strategies.PostponePullerStrategyStrategy);
         History = new ContentPuller(strategies.HistoryPullerStrategyStrategy);

         Postponed.PullCompleted += onPostponedPullCompleted;

         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(() => {
            Debug.WriteLine($"REFRESH FROM {Thread.CurrentThread.ManagedThreadId} THREAD");
            SharedPull();
         });

         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Subscribe(item => {
            Debug.WriteLine($"HISTORY TIMER START IN {Thread.CurrentThread.ManagedThreadId} THREAD");

            if (_historyPullTimer != null) {
               _historyPullTimer.Stop();
               _historyPullTimer.Elapsed -= onTimerElapsed;
               _historyPullTimer.Dispose();
            }

            _historyPullTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            _historyPullTimer.Start();
            _historyPullTimer.Elapsed += onTimerElapsed;
         });

         _eventAggregator.GetEvent<MainBottomEvents.Back>().Subscribe(() => {
            Debug.WriteLine("HISTORY TIMER DISPOSE");
            _historyPullTimer?.Dispose();
         });

      }

      private async void onPostponedPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!e.Successful) return;
      }

      private void onTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs) {
         _currentDispatcher.Invoke(async () => {
            Debug.WriteLine($"HISTORY TIMER TICK IN {Thread.CurrentThread.ManagedThreadId} THREAD");
            if (!_settings.History.Use) {
               return;
            }

            try {
               await History.PullAsync();
            }
            catch (Exception ex) {
               Debug.WriteLine(ex.ToString());
               Debug.WriteLine(ex.Message);
               Debug.WriteLine(ex.StackTrace);
               //ignore
            }
         });
      }

      public void SharedPull() {
         //fire and forget
         /*await*/
         Postponed.PullAsync();
         /*await*/ Actual.PullAsync();

         if (_settings.History.Use) {
#pragma warning disable 4014
            History.PullAsync();
#pragma warning restore 4014
         }
      }
   }
}