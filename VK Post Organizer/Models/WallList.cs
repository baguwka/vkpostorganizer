using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.Models {
   [UsedImplicitly]
   public class WallList : BindableBase {
      private SmartCollection<WallItem> _items;
      public event EventHandler<WallItem> ItemClicked;

      public SmartCollection<WallItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public WallList() {
         Items = new SmartCollection<WallItem>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Items));
      }

      public void Add(WallItem item) {
         _items.Add(item);
      }

      public void Fill(IEnumerable<WallItem> items) {
         _items.Clear();
         _items.AddRange(items);
      }

      public void Clear() {
         _items.Clear();
      }

      public WallItem InstantiateItem(IWallHolder wallHolder) {
         return new WallItem(wallHolder) {
            ClickHandler = OnItemClicked
      };
   }

      protected void OnItemClicked(WallItem e) {
         ItemClicked?.Invoke(this, e);
      }
   }
}
