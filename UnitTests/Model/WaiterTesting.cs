using System;
using System.Threading.Tasks;
using NUnit.Framework;
using vk.Models;

namespace UnitTests.Model {
   [TestFixture]
   public class WaiterTesting {
      [Test]
      public async Task FailToWait() {
         var isBusy = true;

         var result = await Waiter.WaitUntilConditionSetsTrue(() => !isBusy, 5, TimeSpan.FromSeconds(0.5f));
         Assert.That(result, Is.False);
      }

      [Test]
      public async Task SuccessfulyWait() {
         var isBusy = false;

         var result = await Waiter.WaitUntilConditionSetsTrue(() => !isBusy, 5, TimeSpan.FromSeconds(0.5f));
         Assert.That(result, Is.True);
      }

      [Test]
      public async Task SuccessfulyWait2() {
         var isBusy = true;
         Task.Run(async () => {
            await Task.Delay(TimeSpan.FromSeconds(1.2f));
            isBusy = false;
         });

         var result = await Waiter.WaitUntilConditionSetsTrue(() => !isBusy, 5, TimeSpan.FromSeconds(0.5f));
         Assert.That(result, Is.True);
      }
   }
}