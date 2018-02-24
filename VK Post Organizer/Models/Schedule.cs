using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Prism.Mvvm;

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

      public ObservableCollection<ScheduleItem> Items { get; }

      public Schedule() {
          var schedule = new List<ScheduleItem>();

          for (int hh = 9; hh < 22; hh++)
          {
              for (int mm = 0; mm < 59; mm += 10)
              {
                  schedule.Add(new ScheduleItem(hh, mm));
              }
          }
          schedule.Add(new ScheduleItem(9, 05));
          schedule.Add(new ScheduleItem(11, 05));
          schedule.Add(new ScheduleItem(15, 05));
          schedule.Add(new ScheduleItem(18, 05));
          schedule.Add(new ScheduleItem(21, 05));

          schedule = schedule.OrderBy(ToTimeSpan).ToList();
          Items = new ObservableCollection<ScheduleItem>(schedule);
      }

        private static TimeSpan ToTimeSpan(ScheduleItem left)
        {
            return TimeSpan.FromHours(left.Hour).Add(TimeSpan.FromMinutes(left.Minute));
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