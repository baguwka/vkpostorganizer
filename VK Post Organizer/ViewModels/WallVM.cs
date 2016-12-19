using System;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using vk.Models;
using vk.Models.Filter;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.ViewModels {
   [UsedImplicitly]
   public class WallVM : BindableBase {
      private IWallHolder _wallHolder;
      private SmartCollection<PostItem> _items;

      public WallVM([NotNull] IWallHolder wallHolder) {
         if (wallHolder == null) {
            throw new ArgumentNullException(nameof(wallHolder));
         }

         _wallHolder = wallHolder;
         Items = new SmartCollection<PostItem>();

         ExpandAllCommand = new DelegateCommand(expandAllCommandExecute);
         CollapseAllCommand = new DelegateCommand(collapseAllCommandExecute);
      }

      public IWallHolder WallHolder {
         get { return _wallHolder; }
         set { SetProperty(ref _wallHolder, value); }
      }

      public SmartCollection<PostItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public ICommand ExpandAllCommand { get; set; }
      public ICommand CollapseAllCommand { get; set; }

      public void Pull() {
         if (WallHolder == null) {
            return;
         }

         Pull(WallHolder, NoPostFilter.Instance);
      }

      public void Pull([NotNull] IPostFilter filter) {
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }

         if (WallHolder == null) {
            return;
         }

         Pull(WallHolder, filter);
      }

      public void Pull([NotNull] IWallHolder other, [NotNull] IPostFilter filter) {
         if (other == null) {
            throw new ArgumentNullException(nameof(other));
         }
         if (filter == null) {
            throw new ArgumentNullException(nameof(filter));
         }

         WallHolder = other;
         Clear();

         var wall = App.Container.Resolve<WallGet>();

         try {
            var posts1 = wall.Get(WallHolder.ID);
            var posts2 = wall.Get(WallHolder.ID, 50, 100);

            Items.AddRange(filter.FilterPosts(posts1.Response.Wall.Select(p => new PostItem(p))));
            Items.AddRange(filter.FilterPosts(posts2.Response.Wall.Select(p => new PostItem(p))));
         }
         catch (VkException) {
            Clear();
            throw;
         }
      }

      public void Clear() {
         foreach (var postItem in Items) {
            postItem.Clear();
         }
         Items.Clear();
      }

      private void expandAllCommandExecute() {
         foreach (var postItem in Items) {
            postItem.Expand();
         }
      }

      private void collapseAllCommandExecute() {
         foreach (var postItem in Items) {
            postItem.Collapse();
         }
      }
   }
}