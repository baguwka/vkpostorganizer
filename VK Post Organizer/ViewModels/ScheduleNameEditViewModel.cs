using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace vk.ViewModels {
   public class ScheduleNameEditViewModel : BindableBase {
      private string _name;
      public ICommand CancelCommand { get; set; }
      public ICommand OkCommand { get; set; }

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public ScheduleNameEditViewModel() {
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

      public Task OnLoadAsync() {
         throw new NotImplementedException();
      }

      public Task OnClosingAsync() {
         throw new NotImplementedException();
      }
   }
}
