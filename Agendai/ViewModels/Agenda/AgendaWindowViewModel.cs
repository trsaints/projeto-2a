using Agendai.Data.Models;
using Agendai.Data.Converters;
using Agendai.Messages;
using Agendai.Services.Views;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Agendai.Data.Abstractions;

namespace Agendai.ViewModels.Agenda;

public class AgendaWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    private int _selectedIndex;
    private string[] _selectedListNames = [];
    private bool _showData = true;
    private string _selectedMonth;
    private string _selectedWeek;
    private string _selectedDay;
    private DateTime _currentMonth = DateTime.Today;
    private DateTime _currentWeek = DateTime.Today;
    private DateTime _currentDay = DateTime.Today;

    public AgendaWindowViewModel(HomeWindowViewModel? homeWindowVm,
        DateTime? specificDay = null,
        int selectedIndex = 0)
    {
        MonthController = new AgendaMonthController(this);
        WeekController = new AgendaWeekController(this);
        DayController = new AgendaDayController(this);

        SelectedIndex = selectedIndex;

        if (specificDay is not null)
            CurrentDay = specificDay.Value;

        UpdateDateSelectors();
        UpdateDataGridItems();

        if (homeWindowVm is not null)
        {
            HomeWindowVm = homeWindowVm;
            TodoList = HomeWindowVm.TodoWindowVm;
            EventList = HomeWindowVm.EventListVm;
        }

        SubscribeToCollectionChanges();
        RegisterMessages();

        EventList.OnEventAddedOrUpdated = () =>
        {
            EventList.OpenAddEvent = false;
            UpdateDataGridItems();
        };

        TodoList.OnTaskAdded = () =>
        {
            TodoList.OpenAddTask = false;
            UpdateDataGridItems();
        };
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    public string[] Days { get; } =
    [
        "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"
    ];

    public string[] Hours { get; } =
    [
        "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00",
        "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00",
        "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
    ];

    public string Title { get; set; } = "Agenda";

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

    public bool ShowData
    {
        get => _showData;
        set
        {
            if (_showData != value)
            {
                _showData = value;
                OnPropertyChanged();
                UpdateDataGridItems();
            }
        }
    }

    public string SelectedMonth
    {
        get => _selectedMonth;
        set => SetProperty(ref _selectedMonth, value);
    }

    public string SelectedWeek
    {
        get => _selectedWeek;
        set => SetProperty(ref _selectedWeek, value);
    }

    public string SelectedDay
    {
        get => _selectedDay;
        set => SetProperty(ref _selectedDay, value);
    }

    public DateTime CurrentMonth
    {
        get => _currentMonth;
        set => SetProperty(ref _currentMonth, value);
    }

    public DateTime CurrentWeek
    {
        get => _currentWeek;
        set => SetProperty(ref _currentWeek, value);
    }

    public DateTime CurrentDay
    {
        get => _currentDay;
        set => SetProperty(ref _currentDay, value);
    }

    public ObservableCollection<MonthRow> MonthViewRows { get; } = [];
    public ObservableCollection<WeekRow> WeekViewRows { get; } = [];
    public ObservableCollection<DayRow> DayViewRows { get; } = [];

    public EventListViewModel EventList { get; set; } = new();
    public TodoWindowViewModel TodoList { get; set; } = new();

    public AgendaMonthController MonthController { get; }
    public AgendaWeekController WeekController { get; }
    public AgendaDayController DayController { get; }

    public HomeWindowViewModel HomeWindowVm { get; set; }


    public void GoToPreviousMonth() => MonthController.GoToPreviousMonth();
    public void GoToNextMonth() => MonthController.GoToNextMonth();

    public void GoToPreviousWeek() => WeekController.GoToPreviousWeek();
    public void GoToNextWeek() => WeekController.GoToNextWeek();

    public void GoToPreviousDay() => DayController.GoToPreviousDay();
    public void GoToNextDay() => DayController.GoToNextDay();
    public void GoToDay(int date) => DayController.GoToDay(date);

    public void ToggleShowData() => ShowData = !ShowData;

    public void EditEvent(Event evt)
    {
        EventList.OpenAddEvent = true;
        EventList.NewEventName = evt.Name;
        EventList.NewDescription = evt?.Description ?? string.Empty;
        EventList.NewDue = evt!.Due.Date;
        EventList.Repeat = evt.Repeats;
        EventList.LoadEvent(evt);
    }

    public void EditTodo(Todo todo)
    {
        var originalTodo = TodoList.Todos.FirstOrDefault(t => t.Id == todo.Id);

        if (originalTodo is null) return;

        TodoList.OpenAddTask = true;
        TodoList.NewTaskName = todo.Name;
        TodoList.NewDescription = todo?.Description ?? string.Empty;
        TodoList.NewDue = todo!.Due.Date;
        TodoList.SelectedRepeats = TodoList.RepeatOptions
            .FirstOrDefault(o => o.Repeats == todo.Repeats)!;
        TodoList!.ListName = todo?.ListName ?? string.Empty;
        TodoList.EditingTodo = originalTodo;
    }

    public void UpdateDataGridItems()
    {
        switch (SelectedIndex)
        {
            case 0:
                MonthViewService.GenerateMonthView(MonthViewRows,
                    EventList.Events,
                    TodoList.Todos,
                    CurrentMonth,
                    ShowData,
                    SelectedListNames);

                break;

            case 1:
                WeekViewService.GenerateWeekView(WeekViewRows,
                    Hours,
                    EventList.Events,
                    TodoList.Todos,
                    CurrentWeek,
                    ShowData,
                    SelectedListNames);

                break;

            case 2:
                var map = DayViewService.MapDayItemsFrom(EventList.Events,
                    TodoList.Todos,
                    CurrentDay,
                    SelectedListNames);

                DayViewService.GenerateDayView(DayViewRows,
                    Hours,
                    map,
                    ShowData);

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
        CultureInfo culture = new("pt-BR");

        SelectedMonth = culture.TextInfo
            .ToTitleCase(CurrentDay.ToString("MMMM", culture));

        var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(CurrentDay);

        SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";

        SelectedDay = culture.TextInfo.ToTitleCase(CurrentDay.ToString("dddd, dd 'de' MMMM", culture));

        CurrentMonth = CurrentDay;
        CurrentWeek = CurrentDay;
    }

    protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected new void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private void SubscribeToCollectionChanges()
    {
        void Subscribe(INotifyPropertyChanged item)
        {
            item.PropertyChanged += (s, e) => UpdateDataGridItems();
        }

        EventList.Events.CollectionChanged += (s, e) =>
        {
            if (e.NewItems is not null)
                foreach (INotifyPropertyChanged item in e.NewItems) Subscribe(item);

            if (e.OldItems is not null)
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= (s2, e2) => UpdateDataGridItems();

            UpdateDataGridItems();
        };

        TodoList.Todos.CollectionChanged += (s, e) =>
        {
            if (e.NewItems is not null)
                foreach (INotifyPropertyChanged item in e.NewItems) Subscribe(item);

            if (e.OldItems is not null)
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= (s2, e2) => UpdateDataGridItems();

            UpdateDataGridItems();
        };

        foreach (var item in EventList.Events)
            Subscribe(item);

        foreach (var item in TodoList.Todos)
            Subscribe(item);
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default
            .Register<GetListsNamesMessenger>(this, (r, m) =>
            {
                SelectedListNames = m.SelectedItemsName;

                UpdateDataGridItems();
            });
    }
}
