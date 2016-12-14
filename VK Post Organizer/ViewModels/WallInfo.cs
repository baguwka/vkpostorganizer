using System;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallInfo : BindableBase {
      private bool _showPhotos;
      private IWallHolder _wallHolder;
      private SmartCollection<PostItem> _items;

      public WallInfo() {
         WallHolder = App.Container.Resolve<EmptyWallHolder>();
         Items = new SmartCollection<PostItem>();
      }

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public SmartCollection<PostItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public bool ShowPhotos {
         get { return _showPhotos; }
         set { SetProperty(ref _showPhotos, value); }
      }

      public void Load(IWallHolder other) {
         WallHolder = other;
         Items.Clear();

         var wall = App.Container.Resolve<WallGet>();

         try {
            var posts1 = wall.Get(WallHolder.ID);
            var posts2 = wall.Get(WallHolder.ID, 50, 100);

            Items.AddRange(posts1.Response.Wall.Select(p => new PostItem(p)));
            Items.AddRange(posts2.Response.Wall.Select(p => new PostItem(p)));
         }
         catch (VkException ex) {
            Items.Clear();
            throw ex;
         }
      }

      public void Clear() {
         Items.Clear();
      }
   }
}