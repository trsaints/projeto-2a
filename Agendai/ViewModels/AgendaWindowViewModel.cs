using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Agendai.Controllers;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;
using Agendai.Messages;
using Agendai.Services.Views;
using CommunityToolkit.Mvvm.Messaging;


namespace Agendai.ViewModels;

public class AgendaWindowViewModel : ViewModelBase
{
	#region View-Model State

	private string   _selectedMonth = string.Empty;
	private string   _selectedWeek  = string.Empty;
	private string   _selectedDay   = string.Empty;
	private DateTime _currentMonth  = DateTime.Today;
	private DateTime _currentWeek   = DateTime.Today;
	private DateTime _currentDay    = DateTime.Today;
	private string   _searchText    = string.Empty;

	private ObservableCollection<string> _searchableItems = [];

	private int      _selectedIndex;
	private string[] _selectedListNames = [];
	private bool     _showData          = true;

	#endregion


	#region Dependencies

	public           EventListViewModel?   EventList { get; set; } = new();
	public           TodoWindowViewModel?  TodoList  { get; set; } = new();
	private readonly AgendaMonthController _monthController;
	private readonly AgendaWeekController  _weekController;
	public readonly  AgendaDayController   DayController;

	#endregion


	public HomeWindowViewModel? HomeWindowVm { get; set; }

	public AgendaWindowViewModel(
		HomeWindowViewModel? homeWindowVm,
		DateTime?            specificDay   = null,
		int                  selectedIndex = 0
	)
	{
		_monthController = new AgendaMonthController(this);
		_weekController  = new AgendaWeekController(this);
		DayController    = new AgendaDayController(this);

		SelectedIndex = selectedIndex;

		if (specificDay != null)
			CurrentDay = specificDay.Value;

		UpdateDateSelectors();
		UpdateDataGridItems();

		if (homeWindowVm is not null)
		{
			HomeWindowVm = homeWindowVm;
			TodoList     = HomeWindowVm.TodoWindowVm;
			EventList    = HomeWindowVm.EventListVm;
		}

		SubscribeToCollectionChanges();
		RegisterMessages();

		if (EventList is not null)
			EventList.OnEventAddedOrUpdated = () =>
			{
				EventList.OpenAddEvent = false;
				UpdateDataGridItems();
			};

		if (TodoList is not null)
			TodoList.OnTaskAdded = () =>
			{
				TodoList.OpenAddTask = false;
				UpdateDataGridItems();
			};
	}

	public static string[] Hours =>
	[
		"00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00",
		"08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00",
		"16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
	];


	#region State Tracking

	public int SelectedIndex
	{
		get => _selectedIndex;

		set
		{
			if (_selectedIndex == value) return;

			_selectedIndex = value;
			OnPropertyChanged();
			UpdateDataGridItems();
		}
	}

	public string[] SelectedListNames
	{
		get => _selectedListNames;

		private set
		{
			if (_selectedListNames.SequenceEqual(value)) return;

			_selectedListNames = value;
			OnPropertyChanged();
			UpdateDataGridItems();
		}
	}

	public bool ShowData
	{
		get => _showData;

		private set
		{
			if (_showData == value) return;

			_showData = value;
			OnPropertyChanged();
			UpdateDataGridItems();
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

	public ObservableCollection<string> SearchableItems
	{
		get => _searchableItems;
		set => SetProperty(ref _searchableItems, value);
	}

	public string SearchText
	{
		get => _searchText;

		set
		{
			if (SetProperty(ref _searchText, value)) { UpdateDataGridItems(); }
		}
	}


	public ObservableCollection<MonthRow> MonthViewRows { get; } = [];
	public ObservableCollection<WeekRow>  WeekViewRows  { get; } = [];
	public ObservableCollection<DayRow>   DayViewRows   { get; } = [];

	#endregion


	#region Event Handlers

	public void GoToPreviousMonth() => _monthController.GoToPreviousMonth();
	public void GoToNextMonth()     => _monthController.GoToNextMonth();

	public void GoToPreviousWeek() => _weekController.GoToPreviousWeek();
	public void GoToNextWeek()     => _weekController.GoToNextWeek();

	public void GoToPreviousDay() => DayController.GoToPreviousDay();
	public void GoToNextDay()     => DayController.GoToNextDay();
	public void GoToDay(int date) => DayController.GoToDay(date);

	public void ToggleShowData() => ShowData = !ShowData;

	public void EditEvent(Event ev)
	{
		if (EventList is null) return;

		EventList.OpenAddEvent   = true;
		EventList.NewEventName   = ev.Name;
		EventList.NewDescription = ev.Description ?? string.Empty;
		EventList.NewDue         = ev.Due.Date;
		EventList.Repeat =
				EventList.RepeatOptions.FirstOrDefault(o => o.Repeats == ev.Repeats)!;
		EventList.LoadEvent(ev);
	}

	public void EditTodo(Todo todo)
	{
		var originalTodo = TodoList?.Todos.FirstOrDefault(t => t.Id == todo.Id);

		if (originalTodo is null) return;

		if (TodoList is null) return;

		TodoList.OpenAddTask    = true;
		TodoList.NewTaskName    = todo.Name;
		TodoList.NewDescription = todo.Description ?? string.Empty;
		TodoList.NewDue         = todo.Due.Date;
		TodoList.SelectedRepeats =
				TodoList.RepeatOptions.FirstOrDefault(o => o.Repeats == todo.Repeats)!;
		TodoList.ListName    = todo.ListName ?? string.Empty;
		TodoList.EditingTodo = originalTodo;
	}

	public void UpdateDataGridItems()
	{
		switch (SelectedIndex)
		{
			case 0:
				if (EventList is not null && TodoList is not null)
				{
					var newReferenceDateMonth = MonthViewService.GenerateMonthView(
						MonthViewRows,
						EventList.Events,
						TodoList.Todos,
						CurrentMonth,
						ShowData,
						SelectedListNames,
						SearchText
					);
					CurrentMonth = newReferenceDateMonth;
				}

				UpdateDateSelectors();

				break;

			case 1:
				if (EventList is not null && TodoList is not null)
				{
					var newReferenceDateWeek = WeekViewService.GenerateWeekView(
						WeekViewRows,
						Hours,
						EventList.Events,
						TodoList.Todos,
						CurrentWeek,
						ShowData,
						SelectedListNames,
						SearchText
					);
					CurrentWeek = newReferenceDateWeek;
				}

				UpdateDateSelectors();

				break;

			case 2:
				if (EventList is not null && TodoList is not null)
				{
					CurrentDay = DayViewService.GenerateDayView(
						DayViewRows,
						Hours,
						EventList.Events,
						TodoList.Todos,
						CurrentDay,
						ShowData,
						SelectedListNames,
						SearchText
					);
				}

				UpdateDateSelectors();

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
		var culture = new CultureInfo("pt-BR");

		SelectedMonth = culture.TextInfo.ToTitleCase(
			SelectedIndex == 0
					? CurrentMonth.ToString("MMMM", culture)
					: CurrentDay.ToString("MMMM", culture)
		);

		var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(
			SelectedIndex == 1 ? CurrentWeek : CurrentDay
		);

		SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";

		SelectedDay = culture.TextInfo.ToTitleCase(
			CurrentDay.ToString("dddd, dd 'de' MMMM", culture)
		);
	}

	private void SubscribeToCollectionChanges()
	{
		if (EventList is not null)
		{
			EventList.Events.CollectionChanged += (_, e) =>
			{
				if (e.NewItems is not null)
					foreach (INotifyPropertyChanged item in e.NewItems)
						Subscribe(item);
				if (e.OldItems is not null)
					foreach (INotifyPropertyChanged item in e.OldItems)
						item.PropertyChanged -= (_, _) => UpdateSearchableItems();

				UpdateDataGridItems();
				UpdateSearchableItems(); // <== Atualiza aqui
			};

			if (TodoList is not null)
				TodoList.Todos.CollectionChanged += (_, e) =>
				{
					if (e.NewItems is not null)
						foreach (INotifyPropertyChanged item in e.NewItems)
							Subscribe(item);

					if (e.OldItems is not null)
						foreach (INotifyPropertyChanged item in e.OldItems)
							item.PropertyChanged -= (_, _) => UpdateSearchableItems();

					UpdateDataGridItems();
					UpdateSearchableItems(); // <== Atualiza aqui
				};

			foreach (var ev in EventList.Events) Subscribe(ev);
		}

		if (TodoList?.Todos is not null)
			foreach (var todo in TodoList.Todos)
				Subscribe(todo);

		UpdateSearchableItems();

		return;

		void Subscribe(INotifyPropertyChanged item)
		{
			item.PropertyChanged += (_, _) => UpdateSearchableItems();
		}
	}


	private void RegisterMessages()
	{
		WeakReferenceMessenger.Default.Register<GetListsNamesMessenger>(
			this,
			(_, m) =>
			{
				SelectedListNames = m.SelectedItemsName;
				UpdateDataGridItems();
			}
		);
	}

	private void UpdateSearchableItems()
	{
		var names = new HashSet<string>();

		if (EventList is not null)
			foreach (var ev in EventList.Events)
				if (!string.IsNullOrEmpty(ev.Name))
					names.Add(ev.Name);

		if (TodoList is not null)
			foreach (var todo in TodoList.Todos)
				if (!string.IsNullOrEmpty(todo.Name))
					names.Add(todo.Name);

		SearchableItems = new ObservableCollection<string>(names.OrderBy(n => n));
	}

	#endregion
}
