using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;

namespace vk.Models {
   [Serializable]
   public class ScheduleItem : BindableBase {
      private int _hour;
      private int _minute;

      public ScheduleItem(int hour, int minute) {
         Hour = hour;
         Minute = minute;
      }

      public int Hour {
         get { return _hour; }
         set {
            if (value < 0) value = 0;
            if (value > 23) value = 23;
            SetProperty(ref _hour, value);
         }
      }

      public int Minute {
         get { return _minute; }
         set {
            if (value < 0) value = 0;
            if (value > 59) value = 59;
            SetProperty(ref _minute, value);
         }
      }

      public override string ToString() {
         return $"{Hour : 00} : {Minute : 00}";
      }

      protected bool Equals(ScheduleItem other) {
         return _hour == other._hour && _minute == other._minute;
      }

      public override bool Equals(object obj) {
         if (ReferenceEquals(null, obj)) {
            return false;
         }
         if (ReferenceEquals(this, obj)) {
            return true;
         }
         if (obj.GetType() != this.GetType()) {
            return false;
         }
         return Equals((ScheduleItem)obj);
      }

      public override int GetHashCode() {
         unchecked {
            return (_hour * 397) ^ _minute;
         }
      }

      public ScheduleItem() {
      }

      public ScheduleItem(DateTime time) {
         Hour = time.Hour;
         Minute = time.Minute;
      }
   }

   public class Schedule : BindableBase {
      private string _name;

      public string Name {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      public SmartCollection<ScheduleItem> Items { get; }

      public Schedule() {
         Items = new SmartCollection<ScheduleItem> {
            new ScheduleItem(9, 00),
            new ScheduleItem(9, 05),
            new ScheduleItem(9, 10),
            new ScheduleItem(11, 00),
            new ScheduleItem(11, 05),
            new ScheduleItem(11, 10),
            new ScheduleItem(15, 00),
            new ScheduleItem(15, 05),
            new ScheduleItem(15, 10),
            new ScheduleItem(18, 00),
            new ScheduleItem(18, 05),
            new ScheduleItem(18, 10),
            new ScheduleItem(21, 00),
            new ScheduleItem(21, 05),
            new ScheduleItem(21, 10)
         };
      }

      public ScheduleSaveData SaveSchedule() {
         return new ScheduleSaveData(this);
      }

      public void LoadSchedule(ScheduleSaveData data) {
         Items.Clear();

         Name = data.Name;
         Items.AddRange(data.Items);
      }
   }

   [Serializable]
   public class ScheduleSaveData {
      [NotNull]
      public List<ScheduleItem> Items { get; }
      public string Name { get; }

      // ReSharper disable once NotNullMemberIsNotInitialized
      private ScheduleSaveData() {}

      public ScheduleSaveData([NotNull] Schedule schedule) {
         Items = new List<ScheduleItem>();

         Name = schedule.Name;
         Items.AddRange(schedule.Items);
      }
   }
}