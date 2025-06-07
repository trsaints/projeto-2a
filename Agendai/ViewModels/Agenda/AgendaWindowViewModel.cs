using Agendai.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Agendai.Services.Views;
using Agendai.Data.Repositories.Interfaces;

namespace Agendai.ViewModels.Agenda;

public class AgendaWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly ITodoRepository _todoRepository;
    private int _selectedIndex;

    public AgendaWindowViewModel(ITodoRepository todoRepository) {
        _todoRepository = todoRepository;
        TodoViewModel = new TodoWindowViewModel(todoRepository);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title { get; set; } = "Agenda";

    public string[] Days { get; set; } =
    [
        "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"
    ];

    public string[] Hours { get; set; } =
    [
        "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00",
        "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00",
        "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
    ];

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

    public ObservableCollection<MonthRow> MonthViewRows { get; set; } = [];
    public ObservableCollection<WeekRow> WeekViewRows { get; set; } = [];
    public ObservableCollection<DayRow> DayViewRows { get; set; } = [];

    public EventListViewModel EventList { get; set; } = new();
    public TodoWindowViewModel TodoViewModel { get; set; }

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


    public AgendaWindowViewModel(DateTime? specificDay = null, int selectedIndex = 0)
    {
        MonthController = new AgendaMonthController(this);
        WeekController = new AgendaWeekController(this);
        DayController = new AgendaDayController(this);

        SelectedIndex = selectedIndex;

        if (specificDay != null)
            CurrentDay = specificDay.Value;

        UpdateDateSelectors();
        UpdateDataGridItems();
    }


    public void UpdateDataGridItems()
    {
        switch (_selectedIndex)
        {
            case 0:
                MonthViewService.GenerateMonthView(MonthViewRows, EventList.Events, TodoViewModel.Todos, CurrentMonth, _showData);
                break;
            case 1:
                WeekViewService.GenerateWeekView(WeekViewRows, Hours, EventList.Events, TodoViewModel.Todos, CurrentWeek, _showData);
                break;
            case 2:
                var map = DayViewService.MapDayItemsFrom(EventList.Events, TodoViewModel.Todos, CurrentDay);
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

        // Usa a data já atribuída (CurrentDay), ao invés de DateTime.Today
        SelectedMonth = culture.TextInfo.ToTitleCase(CurrentDay.ToString("MMMM", culture));

        var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(CurrentDay);
        SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";

        SelectedDay = culture.TextInfo.ToTitleCase(CurrentDay.ToString("dddd, dd 'de' MMMM", culture));

        CurrentMonth = CurrentDay;
        CurrentWeek = CurrentDay;
    }


    protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected new void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}
