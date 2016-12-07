using System;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.Models {
   public class GroupItem : BindableBase {
      private string _content;
      private int _id;

      public ICommand ClickCommand { get; set; }

      public Action<GroupItem> ClickHandler { get; set; }

      public string Content {
         get { return _content; }
         set { SetProperty(ref _content, value); }
      }

      public int ID {
         get { return _id; }
         private set { _id = value; }
      }

      public GroupItem(int id) {
         ClickCommand = new DelegateCommand(clickCommandExecute);
         _id = id;
      }

      public GroupItem(int id, string content) : this(id) {
         _content = content;
      }

      private void clickCommandExecute() {
         ClickHandler?.Invoke(this);
      }
   }
}