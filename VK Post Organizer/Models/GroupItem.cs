using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace vk.Models {
   public class GroupItem : BindableBase {
      private SmartCollection<Post> _items;
      private Group _groupRef;

      public ICommand ClickCommand { get; set; }

      public Action<GroupItem> ClickHandler { get; set; }

      public SmartCollection<Post> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public Group GroupRef {
         get { return _groupRef; }
         set { SetProperty(ref _groupRef, value); }
      }

      public GroupItem(Group group) {
         Items = new SmartCollection<Post>();
         ClickCommand = new DelegateCommand(clickCommandExecute);
         GroupRef = group;
      }

      private void clickCommandExecute() {
         ClickHandler?.Invoke(this);
      }

      public void Load(Group other) {
         GroupRef = other;
         Items.Clear();

         var wall = App.Container.Resolve<WallGet>();
         var posts1 = wall.Get(GroupRef.ID);
         var posts2 = wall.Get(GroupRef.ID, 50, 100);

         foreach (var post in posts1.Response.Wall.Where(p => p.Attachments.Count > 0)) {
            post.Text = post.Attachments.FirstOrDefault(a => a.Type == "photo")?.Photo.Photo1280;
         }

         foreach (var post in posts2.Response.Wall.Where(p => p.Attachments.Count > 0)) {
            post.Text = post.Attachments.FirstOrDefault(a => a.Type == "photo")?.Photo.Photo1280;
         }

         Items.AddRange(posts1.Response.Wall);
         Items.AddRange(posts2.Response.Wall);
      }
   }
}