using Agendai.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Agendai.Messages;
using Agendai.Services.Views;
using Agendai.Views.Components.EventList;
using CommunityToolkit.Mvvm.Messaging;

namespace Agendai.ViewModels.Agenda
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
        public TodoWindowViewModel TodoList { get; set; } = new();

        public AgendaMonthController MonthController { get; }
        public AgendaWeekController WeekController { get; }
        public AgendaDayController DayController { get; }

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
        public DateTime CurrentMonth
        {
            get => _currentMonth;
            set => SetProperty(ref _currentMonth, value);
        }

        private DateTime _currentWeek = DateTime.Today;
        public DateTime CurrentWeek
        {
            get => _currentWeek;
            set => SetProperty(ref _currentWeek, value);
        }

        private DateTime _currentDay = DateTime.Today;
        public DateTime CurrentDay
        {
            get => _currentDay;
            set => SetProperty(ref _currentDay, value);
        }
        
        private string[] _selectedListNames = Array.Empty<string>();

        public string[] SelectedListNames
        {
            get => _selectedListNames;
            set
            {
                if (!_selectedListNames.SequenceEqual(value))
                {
                    _selectedListNames = value;
                    OnPropertyChanged();
                    UpdateDataGridItems();
                }
            }
        }


        
        private bool _showData = true;
        public bool ShowData
        {
            get => _showData;
            set
            {
                if (_showData != value)
                {
                    _showData = value;
                    OnPropertyChanged(nameof(ShowData));
                    UpdateDataGridItems();
                }
            }
        }

        public void ToggleShowData()
        {
            ShowData = !ShowData;
        }
        
        public void GoToPreviousMonth() => MonthController.GoToPreviousMonth();
        public void GoToNextMonth() => MonthController.GoToNextMonth();

        public void GoToPreviousWeek() => WeekController.GoToPreviousWeek();
        public void GoToNextWeek() => WeekController.GoToNextWeek();

        public void GoToPreviousDay() => DayController.GoToPreviousDay();
        public void GoToNextDay() => DayController.GoToNextDay();
        public void GoToDay(int date) => DayController.GoToDay(date);
        
        public HomeWindowViewModel HomeWindowVm { get; set; }
        
        public AgendaWindowViewModel(HomeWindowViewModel? homeWindowVm, DateTime? specificDay = null, int selectedIndex = 0)
        {
            MonthController = new AgendaMonthController(this);
            WeekController = new AgendaWeekController(this);
            DayController = new AgendaDayController(this);

            SelectedIndex = selectedIndex;

            if (specificDay != null)
                CurrentDay = specificDay.Value;

            UpdateDateSelectors();
            UpdateDataGridItems();
            if (homeWindowVm != null)
            {
                HomeWindowVm = homeWindowVm;
                TodoList = HomeWindowVm.TodoWindowVm;
                EventList = HomeWindowVm.EventListVm;
            }

            EventList.Events.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += (s2, e2) => UpdateDataGridItems();
                }
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= (s2, e2) => UpdateDataGridItems();
                }
                UpdateDataGridItems();
            };

            TodoList.Todos.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += (s2, e2) => UpdateDataGridItems();
                }
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= (s2, e2) => UpdateDataGridItems();
                }
                UpdateDataGridItems();
            };

            foreach (INotifyPropertyChanged item in EventList.Events)
                item.PropertyChanged += (s2, e2) => UpdateDataGridItems();

            foreach (INotifyPropertyChanged item in TodoList.Todos)
                item.PropertyChanged += (s2, e2) => UpdateDataGridItems();
            
            WeakReferenceMessenger.Default.Register<GetListsNamesMessenger>(this, (r, m) =>
            {
                SelectedListNames = m.SelectedItemsName;
                UpdateDataGridItems();

            });

        }


        public void UpdateDataGridItems()
        {
            switch (_selectedIndex)
            {
                case 0:
                    MonthViewService.GenerateMonthView(MonthViewRows, EventList.Events, TodoList.Todos, CurrentMonth, _showData, SelectedListNames);
                    break;
                case 1:
                    WeekViewService.GenerateWeekView(WeekViewRows, Hours, EventList.Events, TodoList.Todos, CurrentWeek, _showData);
                    break;
                case 2:
                    var map = DayViewService.MapDayItemsFrom(EventList.Events, TodoList.Todos, CurrentDay);
                    DayViewService.GenerateDayView(DayViewRows, Hours, map, _showData);
                    break;
                default:
                    MonthViewRows.Clear();
                    WeekViewRows.Clear();
                    DayViewRows.Clear();
                    break;
            }
        }

        public void UpdateDateSelectors()
        {
            var culture = new CultureInfo("pt-BR");

            SelectedMonth = culture.TextInfo.ToTitleCase(CurrentDay.ToString("MMMM", culture));

            var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(CurrentDay);
            SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";

            SelectedDay = culture.TextInfo.ToTitleCase(CurrentDay.ToString("dddd, dd 'de' MMMM", culture));

            CurrentMonth = CurrentDay;
            CurrentWeek = CurrentDay;
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
