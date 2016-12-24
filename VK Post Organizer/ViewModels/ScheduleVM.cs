using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using vk.Models;

namespace vk.ViewModels {
   public class ScheduleVM : BindableBase, IVM {
      private bool _isAnyItemSelected;
      private ScheduleItem _selectedItem;
      private ScheduleItem _editableItem;
      public ICommand AddTimeCommand { get; set; }
      public ICommand RemoveTimeCommand { get; set; }

      public ICommand ConfirmCommand { get; set; }

      public ICommand AddNameCommand { get; set; }
      public ICommand RemoveNameCommand { get; set; }
      public ICommand EditNameCommand { get; set; }

      public ICommand ApplyCommand { get; set; }

      public bool IsAnyItemSelected {
         get { return _isAnyItemSelected; }
         set { SetProperty(ref _isAnyItemSelected, value); }
      }

      public SmartCollection<ScheduleItem> Items { get; }

      public ScheduleItem SelectedItem {
         get { return _selectedItem; }
         set { SetProperty(ref _selectedItem, value); }
      }

      public ScheduleItem EditableItem {
         get { return _editableItem; }
         set { SetProperty(ref _editableItem, value); }
      }

      public static IEnumerable<int> Hours => Enumerable.Range(0, 24);
      public static IEnumerable<int> Minutes => Enumerable.Range(0, 60);

      public ScheduleVM() {
         EditableItem = new ScheduleItem(12, 00);

         Items = new SmartCollection<ScheduleItem>();
         Items.Add(new ScheduleItem(9, 00));
         Items.Add(new ScheduleItem(9, 05));
         Items.Add(new ScheduleItem(9, 10));
         Items.Add(new ScheduleItem(11, 00));
         Items.Add(new ScheduleItem(11, 05));
         Items.Add(new ScheduleItem(11, 10));
         Items.Add(new ScheduleItem(15, 00));
         Items.Add(new ScheduleItem(15, 05));
         Items.Add(new ScheduleItem(15, 10));
         Items.Add(new ScheduleItem(18, 00));
         Items.Add(new ScheduleItem(18, 05));
         Items.Add(new ScheduleItem(18, 10));
         Items.Add(new ScheduleItem(21, 00));
         Items.Add(new ScheduleItem(21, 05));
         Items.Add(new ScheduleItem(21, 10));

         AddTimeCommand = new DelegateCommand(addTimeCommandExecute);
         RemoveTimeCommand = new DelegateCommand(removeTimeCommandExecute);

         ConfirmCommand = new DelegateCommand(confirmCommandExecute);

         AddNameCommand = new DelegateCommand(addNameCommandExecute);
         RemoveNameCommand = new DelegateCommand(removeNameCommandExecute);
         EditNameCommand = new DelegateCommand(editNameCommandExecute);

         ApplyCommand = new DelegateCommand(ApplyToSelectedItem);
      }

      private void addTimeCommandExecute() {
         Items.Add(new ScheduleItem(0, 0));
         SortList();
      }

      private void removeTimeCommandExecute() {
         if (SelectedItem == null) return;
         var item = Items.FirstOrDefault(i => Equals(i, SelectedItem));
         if (item == null) return;
         Items.Remove(item);
      }

      private void confirmCommandExecute() {
      }

      private void addNameCommandExecute() {
      }

      private void removeNameCommandExecute() {
      }

      private void editNameCommandExecute() {
      }

      public void OnLoad() {
      }

      public void OnClosing() {
      }

      public void OnClosed() {
      }

      public void OnSelectionChange() {
         FromSelectedItem();
      }

      public void ApplyToSelectedItem() {
         if (SelectedItem == null) return;
         SelectedItem.Hour = EditableItem.Hour;
         SelectedItem.Minute = EditableItem.Minute;

         SortList();
      }

      public void SortList() {
         var tempCollection = new List<ScheduleItem>(Items);
         tempCollection.Sort((item1, item2) => (item1.Hour * 60 + item1.Minute).CompareTo(item2.Hour * 60 + item2.Minute));

         var currentlySelected = SelectedItem;

         Items.Clear();
         Items.AddRange(tempCollection);

         SelectedItem = Items.FirstOrDefault(i => Equals(i, currentlySelected));
      }

      public void FromSelectedItem() {
         if (SelectedItem == null) return;

         EditableItem.Hour = SelectedItem.Hour;
         EditableItem.Minute = SelectedItem.Minute;
      }
   }
}
