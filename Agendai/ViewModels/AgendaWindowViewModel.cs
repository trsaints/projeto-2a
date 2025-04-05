using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Agendai.ViewModels
{
    public class AgendaWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get; set; } = "Agenda";

        public string[] Days { get; set; } = new string[]
        {
            "Domingo", "Segunda", "Terça", "Quarta", "Quinta",
            "Sexta", "Sábado"
        };

        public string[] Hours { get; set; } = new string[]
        {
            "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00",
            "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00",
            "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
        };

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                    UpdateDataGridItems();
                }
            }
        }

        public ObservableCollection<MonthRow> MonthViewRows { get; set; } = new();
        public ObservableCollection<WeekRow> WeekViewRows { get; set; } = new();
        public ObservableCollection<DayRow> DayViewRows { get; set; } = new();

        private string _selectedMonth;
        public string SelectedMonth
        {
            get => _selectedMonth;
            set => SetProperty(ref _selectedMonth, value);
        }

        private string _selectedWeek;
        public string SelectedWeek
        {
            get => _selectedWeek;
            set => SetProperty(ref _selectedWeek, value);
        }

        private string _selectedDay;
        public string SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        private Dictionary<(string Hour, string Day), string> Events { get; set; } = new();

        public AgendaWindowViewModel()
        {
            var today = DateTime.Today;
            var culture = new CultureInfo("pt-BR");

            SelectedMonth = culture.TextInfo.ToTitleCase(today.ToString("MMMM", culture));
            SelectedWeek = $"Semana {GetWeekOfMonth(today)}";
            SelectedDay = culture.TextInfo.ToTitleCase(today.ToString("dddd, dd 'de' MMMM", culture));

            AddSampleEvents();
            UpdateDataGridItems();
        }

        private void UpdateDataGridItems()
        {
            switch (_selectedIndex)
            {
                case 0:
                    GenerateMonthView();
                    break;
                case 1:
                    GenerateWeekView();
                    break;
                case 2:
                    GenerateDayView();
                    break;
                default:
                    MonthViewRows.Clear();
                    WeekViewRows.Clear();
                    DayViewRows.Clear();
                    break;
            }
        }

        private void GenerateMonthView()
        {
            MonthViewRows.Clear();
            var today = DateTime.Today;
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            DateTime firstDay = new DateTime(today.Year, today.Month, 1);
            int startOffset = (int)firstDay.DayOfWeek;

            int totalSlots = daysInMonth + startOffset;
            int totalWeeks = (int)Math.Ceiling(totalSlots / 7.0);

            int day = 1;
            for (int w = 0; w < totalWeeks; w++)
            {
                var row = new MonthRow();
                for (int d = 0; d < 7; d++)
                {
                    int slot = w * 7 + d;
                    if (slot < startOffset || day > daysInMonth)
                    {
                        row[d] = "";
                    }
                    else
                    {
                        row[d] = day.ToString();
                        day++;
                    }
                }
                MonthViewRows.Add(row);
            }
        }

        private void GenerateWeekView()
        {
            var today = DateTime.Today;
            SelectedWeek = $"Semana {GetWeekOfMonth(today)}";

            WeekViewRows.Clear();
            foreach (var hour in Hours)
            {
                var row = new WeekRow { Hour = hour };
                foreach (var day in new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" })
                {
                    row[day] = Events.TryGetValue((hour, day), out var evt) ? evt : string.Empty;
                }
                WeekViewRows.Add(row);
            }
        }

        private void GenerateDayView()
        {
            var today = DateTime.Today;
            var culture = new CultureInfo("pt-BR");
            SelectedDay = culture.TextInfo.ToTitleCase(today.ToString("dddd, dd 'de' MMMM", culture));

            DayViewRows.Clear();
            string day = today.ToString("dddd", CultureInfo.CurrentCulture);
            day = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(day);

            foreach (var hour in Hours)
            {
                var row = new DayRow
                {
                    Hour = hour,
                    Event = Events.TryGetValue((hour, day), out var evt) ? evt : string.Empty
                };
                DayViewRows.Add(row);
            }
        }

        private void AddSampleEvents()
        {
            Events[("08:00", "Monday")] = "Team meeting";
            Events[("14:00", "Wednesday")] = "Code review";
            Events[("16:00", "Friday")] = "Project presentation";
        }

        private int GetWeekOfMonth(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            var dayOfMonth = date.Day + firstDayOfWeek;
            return (int)Math.Ceiling(dayOfMonth / 7.0);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }

    public class MonthRow
    {
        public string Sunday { get; set; } = "";
        public string Monday { get; set; } = "";
        public string Tuesday { get; set; } = "";
        public string Wednesday { get; set; } = "";
        public string Thursday { get; set; } = "";
        public string Friday { get; set; } = "";
        public string Saturday { get; set; } = "";

        public string this[int index]
        {
            get => index switch
            {
                0 => Sunday,
                1 => Monday,
                2 => Tuesday,
                3 => Wednesday,
                4 => Thursday,
                5 => Friday,
                6 => Saturday,
                _ => ""
            };
            set
            {
                switch (index)
                {
                    case 0: Sunday = value; break;
                    case 1: Monday = value; break;
                    case 2: Tuesday = value; break;
                    case 3: Wednesday = value; break;
                    case 4: Thursday = value; break;
                    case 5: Friday = value; break;
                    case 6: Saturday = value; break;
                }
            }
        }
    }

    public class WeekRow
    {
        public string Hour { get; set; }
        public string Sunday { get; set; } = "";
        public string Monday { get; set; } = "";
        public string Tuesday { get; set; } = "";
        public string Wednesday { get; set; } = "";
        public string Thursday { get; set; } = "";
        public string Friday { get; set; } = "";
        public string Saturday { get; set; } = "";

        public string this[string day]
        {
            get => day switch
            {
                "Sunday" => Sunday,
                "Monday" => Monday,
                "Tuesday" => Tuesday,
                "Wednesday" => Wednesday,
                "Thursday" => Thursday,
                "Friday" => Friday,
                "Saturday" => Saturday,
                _ => string.Empty
            };
            set
            {
                switch (day)
                {
                    case "Sunday": Sunday = value; break;
                    case "Monday": Monday = value; break;
                    case "Tuesday": Tuesday = value; break;
                    case "Wednesday": Wednesday = value; break;
                    case "Thursday": Thursday = value; break;
                    case "Friday": Friday = value; break;
                    case "Saturday": Saturday = value; break;
                }
            }
        }
    }

    public class DayRow
    {
        public string Hour { get; set; }
        public string Event { get; set; }
    }
}
