using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Prism.Mvvm;

namespace vk.Models {
   [UsedImplicitly]
   public class WallList : BindableBase {
      private ObservableCollection<WallItem> _items;
      public event EventHandler<WallItem> ItemClicked;

      public ObservableCollection<WallItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public WallList() {
         Items = new ObservableCollection<WallItem>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Items));
      }

      public void Add([NotNull] WallItem item) {
         if (item == null) {
            throw new ArgumentNullException(nameof(item));
         }

         _items.Add(item);
         item.Clicked += OnItemClicked;
      }

      public void Remove([NotNull] WallItem item) {
         if (item == null) {
            throw new ArgumentNullException(nameof(item));
         }

         if (_items.Contains(item)) {
            _items.Remove(item);
            item.Clicked -= OnItemClicked;
         }
      }
      
      public void Clear() {
         foreach (var wallItem in _items.ToList()) {
            Remove(wallItem);
         }
      }

      protected void OnItemClicked(object sender, WallItem e) {
         ItemClicked?.Invoke(this, e);
      }
   }
}
