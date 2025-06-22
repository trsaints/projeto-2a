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


namespace Agendai.ViewModels.Agenda;

public class AgendaWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public string[] Days { get; } = new[]
	{
		"Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"
	};

	public string[] Hours { get; } = new[]
	{
		"00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00",
		"08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00",
		"16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"
	};

	public string Title { get; set; } = "Agenda";

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
				OnPropertyChanged();
				UpdateDataGridItems();
			}
		}
	}

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

	private ObservableCollection<string> _searchableItems = new();
	public ObservableCollection<string> SearchableItems
	{
		get => _searchableItems;
		set => SetProperty(ref _searchableItems, value);
	}

	private string _searchText = string.Empty;
	public string SearchText
	{
		get => _searchText;

		set
		{
			if (SetProperty(ref _searchText, value)) { UpdateDataGridItems(); }
		}
	}


	public ObservableCollection<MonthRow> MonthViewRows { get; } = new();
	public ObservableCollection<WeekRow>  WeekViewRows  { get; } = new();
	public ObservableCollection<DayRow>   DayViewRows   { get; } = new();

	public EventListViewModel  EventList { get; set; } = new();
	public TodoWindowViewModel TodoList  { get; set; } = new();

	public AgendaMonthController MonthController { get; }
	public AgendaWeekController  WeekController  { get; }
	public AgendaDayController   DayController   { get; }

	public HomeWindowViewModel HomeWindowVm { get; set; }

	public AgendaWindowViewModel(
		HomeWindowViewModel? homeWindowVm,
		DateTime?            specificDay   = null,
		int                  selectedIndex = 0
	)
	{
		MonthController = new AgendaMonthController(this);
		WeekController  = new AgendaWeekController(this);
		DayController   = new AgendaDayController(this);

		SelectedIndex = selectedIndex;

		if (specificDay != null)
			CurrentDay = specificDay.Value;

		UpdateDateSelectors();
		UpdateDataGridItems();

		if (homeWindowVm != null)
		{
			HomeWindowVm = homeWindowVm;
			TodoList     = HomeWindowVm.TodoWindowVm;
			EventList    = HomeWindowVm.EventListVm;
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

	public void GoToPreviousMonth() => MonthController.GoToPreviousMonth();
	public void GoToNextMonth()     => MonthController.GoToNextMonth();

	public void GoToPreviousWeek() => WeekController.GoToPreviousWeek();
	public void GoToNextWeek()     => WeekController.GoToNextWeek();

	public void GoToPreviousDay() => DayController.GoToPreviousDay();
	public void GoToNextDay()     => DayController.GoToNextDay();
	public void GoToDay(int date) => DayController.GoToDay(date);

	public void ToggleShowData() => ShowData = !ShowData;

	public void EditEvent(Event ev)
	{
		EventList.OpenAddEvent = true;
		EventList.NewEventName = ev.Name;
		EventList.NewDescription = ev.Description;
		EventList.NewDue = ev.Due.Date;
		EventList.Repeat = EventList.RepeatOptions.FirstOrDefault(o => o.Repeats == ev.Repeats);
		EventList.LoadEvent(ev);
	}

	public void EditTodo(Todo todo)
	{
		var originalTodo = TodoList.Todos.FirstOrDefault(t => t.Id == todo.Id);

		if (originalTodo == null) return;

		TodoList.OpenAddTask    = true;
		TodoList.NewTaskName    = todo.Name;
		TodoList.NewDescription = todo.Description;
		TodoList.NewDue         = todo.Due.Date;
		TodoList.SelectedRepeats =
				TodoList.RepeatOptions.FirstOrDefault(o => o.Repeats == todo.Repeats);
		TodoList.ListName    = todo.ListName;
		TodoList.EditingTodo = originalTodo;
	}

	public void UpdateDataGridItems()
	{
		switch (SelectedIndex)
		{
			case 0:
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
				UpdateDateSelectors();

				break;

			case 1:
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
				UpdateDateSelectors();

				break;

			case 2:
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
				UpdateDateSelectors();

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

		if (SelectedIndex == 0)
		{
			SelectedMonth = culture.TextInfo.ToTitleCase(
				CurrentMonth.ToString("MMMM", culture)
			);
		}
		else
		{
			SelectedMonth = culture.TextInfo.ToTitleCase(
				CurrentDay.ToString("MMMM", culture)
			);
		}

		var (weekNumber, start, end) = WeekViewService.GetWeekOfMonthRange(
			SelectedIndex == 1 ? CurrentWeek : CurrentDay
		);
		SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";

		SelectedDay = culture.TextInfo.ToTitleCase(
			(SelectedIndex == 2 ? CurrentDay : CurrentDay)
			.ToString("dddd, dd 'de' MMMM", culture)
		);
	}


	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	protected bool SetProperty<T>(
		ref T                      field,
		T                          value,
		[CallerMemberName] string? propertyName = null
	)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return false;

		field = value;
		OnPropertyChanged(propertyName);

		return true;
	}

	private void SubscribeToCollectionChanges()
	{
		void Subscribe(INotifyPropertyChanged item)
		{
			item.PropertyChanged += (s, e) => UpdateSearchableItems();
		}

		EventList.Events.CollectionChanged += (s, e) =>
		{
			if (e.NewItems != null)
				foreach (INotifyPropertyChanged item in e.NewItems)
					Subscribe(item);
			if (e.OldItems != null)
				foreach (INotifyPropertyChanged item in e.OldItems)
					item.PropertyChanged -= (s2, e2) => UpdateSearchableItems();

			UpdateDataGridItems();
			UpdateSearchableItems(); // <== Atualiza aqui
		};

		TodoList.Todos.CollectionChanged += (s, e) =>
		{
			if (e.NewItems != null)
				foreach (INotifyPropertyChanged item in e.NewItems)
					Subscribe(item);
			if (e.OldItems != null)
				foreach (INotifyPropertyChanged item in e.OldItems)
					item.PropertyChanged -= (s2, e2) => UpdateSearchableItems();

			UpdateDataGridItems();
			UpdateSearchableItems(); // <== Atualiza aqui
		};

		foreach (var ev in EventList.Events) Subscribe(ev);
		foreach (var todo in TodoList.Todos) Subscribe(todo);

		UpdateSearchableItems();
	}


	private void RegisterMessages()
	{
		WeakReferenceMessenger.Default.Register<GetListsNamesMessenger>(
			this,
			(r, m) =>
			{
				SelectedListNames = m.SelectedItemsName;
				UpdateDataGridItems();
			}
		);
	}

	private void UpdateSearchableItems()
	{
		var names = new HashSet<string>();

		foreach (var ev in EventList.Events)
			if (!string.IsNullOrEmpty(ev.Name))
				names.Add(ev.Name);

		foreach (var todo in TodoList.Todos)
			if (!string.IsNullOrEmpty(todo.Name))
				names.Add(todo.Name);

		SearchableItems = new ObservableCollection<string>(names.OrderBy(n => n));
	}
}
