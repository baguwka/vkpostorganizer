//https://peteohanlon.wordpress.com/2008/10/22/bulk-loading-in-observablecollection/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace vk.Models {
   public class RangeObservableCollection<T> : ObservableCollection<T> {
      private bool _suppressNotification = false;

      public RangeObservableCollection() {
      }

      public RangeObservableCollection(List<T> list) : base(list) {
      }

      public RangeObservableCollection([NotNull] IEnumerable<T> collection) : base(collection) {
      }

      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
         if (!_suppressNotification)
            base.OnCollectionChanged(e);
      }

      public void AddRange([NotNull] IEnumerable<T> range) {
         if (range == null) throw new ArgumentNullException(nameof(range));

         _suppressNotification = true;

         foreach (var item in range) {
            Items.Add(item);
         }

         _suppressNotification = false;
         OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
   }
}
