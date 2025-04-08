using Agendai.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Agendai.Services.Views;

namespace Agendai.ViewModels
{
    public class AgendaWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get; set; } = "Agenda";

        public string[] Days { get; set; } = new[]
        {
            "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"
        };

        public string[] Hours { get; set; } = new[]
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
        
        public EventListViewModel EventList { get; set; } = new();
        public TodoListViewModel TodoList { get; set; } = new();

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
        
        private DateTime _currentMonth = DateTime.Today;
        public void GoToPreviousMonth()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateMonthFromDate();
        }

        public void GoToNextMonth()
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateMonthFromDate();
        }

        private void UpdateMonthFromDate()
        {
            var culture = new CultureInfo("pt-BR");
            SelectedMonth = culture.TextInfo.ToTitleCase(_currentMonth.ToString("MMMM", culture));
            UpdateDataGridItems();
        }

        private DateTime _currentWeek = DateTime.Today;
        public void GoToPreviousWeek()
        {
            _currentWeek = _currentWeek.AddDays(-7);
            UpdateWeekFromDate();
        }

        public void GoToNextWeek()
        {
            _currentWeek = _currentWeek.AddDays(7);
            UpdateWeekFromDate();
        }
        
        private void UpdateWeekFromDate()
        {
            var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(_currentWeek);
            SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";
            UpdateDataGridItems();
        }

        public AgendaWindowViewModel()
        {
            UpdateDateSelectors();
            UpdateDataGridItems();
        }

        private void UpdateDataGridItems()
        {
            switch (_selectedIndex)
            {
                case 0:
                    MonthViewService.GenerateMonthView(MonthViewRows, EventList.Events, TodoList.Todos, _currentMonth);
                    break;
                case 1:
                    WeekViewService.GenerateWeekView(WeekViewRows, Hours, EventList.Events, TodoList.Todos, _currentWeek);
                    break;
                case 2:
                    var culture = new CultureInfo("pt-BR");
                    var selected = DateTime.Parse(SelectedDay, culture);
                    var map = DayViewService.MapDayItemsFrom(EventList.Events, TodoList.Todos, selected);
                    DayViewService.GenerateDayView(DayViewRows, Hours, map);
                    break;

                default:
                    MonthViewRows.Clear();
                    WeekViewRows.Clear();
                    DayViewRows.Clear();
                    break;
            }
        }
        
        private void UpdateDateSelectors()
        {
            var today = DateTime.Today;
            var culture = new CultureInfo("pt-BR");

            SelectedMonth = culture.TextInfo.ToTitleCase(today.ToString("MMMM", culture));
            var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(today);
            SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";
            SelectedDay = culture.TextInfo.ToTitleCase(today.ToString("dddd, dd 'de' MMMM", culture));
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
}
