using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models {
   [UsedImplicitly]
   public class GroupCollection : BindableBase {
      private SmartCollection<GroupItem> _items;
      public event EventHandler<GroupItem> ItemClicked;

      public SmartCollection<GroupItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public GroupCollection() {
         Items = new SmartCollection<GroupItem>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Items));
      }

      public void Add(GroupItem item) {
         _items.Add(item);
      }

      public void Fill(IEnumerable<GroupItem> items) {
         _items.Clear();
         _items.AddRange(items);
      }

      public void Clear() {
         _items.Clear();
      }

      public GroupItem InstantiateItem(Group group) {
         return new GroupItem(group) {
            ClickHandler = OnItemClicked
      };
   }

      protected void OnItemClicked(GroupItem e) {
         ItemClicked?.Invoke(this, e);
      }
   }
}
