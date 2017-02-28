using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using JetBrains.Annotations;
using Prism.Events;
using vk.Events;
using vk.Utils;
using vk.ViewModels;
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
            if (_settings.History.Use) {
               History.WallHolder = _sharedWallHolder;
            }
         }
      }

      public PullersController(IEventAggregator eventAggregator, PullerStrategies strategies, Settings settings) {
         _eventAggregator = eventAggregator;
         _settings = settings;

         Actual = new ContentPuller(strategies.ActualPullerStrategyStrategy);
         Postponed = new ContentPuller(strategies.PostponePullerStrategyStrategy);
         History = new ContentPuller(strategies.HistoryPullerStrategyStrategy);

         Postponed.PullCompleted += onPostponedPullCompleted;

         _eventAggregator.GetEvent<MainBottomEvents.Refresh>().Subscribe(async () => {
            Debug.WriteLine($"REFRESH FROM {Thread.CurrentThread.ManagedThreadId} THREAD");
            await SharedPull();
         });

         _eventAggregator.GetEvent<WallSelectorEvents.WallSelected>().Subscribe(item => {
            //Debug.WriteLine($"HISTORY TIMER START IN {Thread.CurrentThread.ManagedThreadId} THREAD");

            //if (_historyPullTimer != null) {
            //   _historyPullTimer.Stop();
            //   _historyPullTimer.Elapsed -= onTimerElapsed;
            //   _historyPullTimer.Dispose();
            //}

            //_historyPullTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            //_historyPullTimer.Start();
            //_historyPullTimer.Elapsed += onTimerElapsed;
         });

         _eventAggregator.GetEvent<MainBottomEvents.Back>().Subscribe(() => {
            Debug.WriteLine("HISTORY TIMER DISPOSE");
            _historyPullTimer?.Dispose();
         });

      }

      private async void onPostponedPullCompleted(object sender, ContentPullerEventArgs e) {
         if (!e.Successful) return;
      }

      private async void onTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs) {
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
      }

      public async Task SharedPull() {
         await Postponed.PullAsync();

         if (_settings.History.Use) {
            History.PullAsync();
         }
      }
   }
}