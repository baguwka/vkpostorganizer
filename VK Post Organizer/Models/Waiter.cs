using System;
using System.Threading.Tasks;

namespace vk.Models {
   public static class Waiter {
      /// <summary>
      /// Waits until condition sets true.
      /// </summary>
      /// <param name="condition">Condition to check</param>
      /// <param name="attempts">Total number of attemts</param>
      /// <param name="interval">Time interval</param>
      /// <returns>Returns true if condition sets true within attempts, returns false if not</returns>
      public static async Task<bool> WaitUntilConditionSetsTrue(Func<bool> condition, int attempts, TimeSpan interval) {
         var currentAttempts = 0;
         while (!condition()) {
            if (currentAttempts > attempts) {
               return false;
            }
            currentAttempts++;
            await Task.Delay(interval);
         }
         return true;
      }
   }
}