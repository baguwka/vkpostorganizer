using System;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.ViewModels {
   public class ScheduleNameEditVM : BindableBase, IVM {
      private string _name;
      public ICommand CancelCommand { get; set; }
      public ICommand OkCommand { get; set; }

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public ScheduleNameEditVM() {
         CancelCommand = new DelegateCommand(cancelCommandExecute);
         OkCommand = new DelegateCommand(okCommandExecute);
      }

      private void okCommandExecute() {
         throw new NotImplementedException();
      }

      private void cancelCommandExecute() {
         throw new NotImplementedException();
      }

      public void OnLoad() {
      }

      public void OnClosing() {
      }

      public void OnClosed() {
      }

   }
}
