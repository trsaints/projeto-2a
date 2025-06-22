using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Agendai.Data;
using Agendai.Data.Models;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class EventListViewModel : ViewModelBase
{
	#region View-Model State

	private Event? _currentEvent;
	private Event? _selectedEvent;
	private bool   _canSave;
	private bool   _isAddTodoPopupOpen;
	private bool   _openAddEvent;

	private string                      _newEventName = string.Empty;
	private DateTime                    _newDue = DateTime.Today;
	private string                      _newDescription = string.Empty;
	private string                      _agendaName = string.Empty;
	private RepeatsOption               _repeat = new() { Repeats = Repeats.None };
	private ObservableCollection<Todo?> _todosForSelectedEvent = [];

	private ObservableCollection<string> _agendaNames;

	#endregion


	#region Constructor

	public EventListViewModel(TodoWindowViewModel? todoWindowVm = null)
	{
		TodoWindowVm = todoWindowVm;

		OpenPopupCommand  = new RelayCommand(() => IsAddTodoPopupOpen = true);
		ClosePopupCommand = new RelayCommand(() => IsAddTodoPopupOpen = false);
		AddEventCommand   = new RelayCommand(AddOrUpdateEvent, () => CanSave);
		CancelCommand     = new RelayCommand(CancelAction);

		OnEventAddedOrUpdated = () => { OpenAddEvent = false; };

		Events = new ObservableCollection<Event>(GenerateSampleEvents());
		_agendaNames =
				new ObservableCollection<string>(Events.Select(e => e.AgendaName).Distinct()!);

		if (TodoWindowVm is not null)
		{
			TodoWindowVm.PropertyChanged += (_, e) =>
			{
				if (e.PropertyName is nameof(TodoWindowVm.NewTaskName)
				                      or nameof(TodoWindowVm.SelectedTodoName))
				{
					UpdateCanSave();
				}
			};
		}
	}

	#endregion


	#region State Tracking

	public ObservableCollection<Event> Events { get; set; }

	public ObservableCollection<RepeatsOption> RepeatOptions { get; } =
	[
		new() { Repeats = Repeats.None }, new() { Repeats   = Repeats.Daily },
		new() { Repeats = Repeats.Weekly }, new() { Repeats = Repeats.Monthly },
		new() { Repeats = Repeats.Anually }
	];

	public ObservableCollection<string> AgendaNames
	{
		get => _agendaNames;
		set => SetProperty(ref _agendaNames, value);
	}

	public bool IsAddTodoPopupOpen
	{
		get => _isAddTodoPopupOpen;
		set => SetProperty(ref _isAddTodoPopupOpen, value);
	}

	public bool OpenAddEvent
	{
		get => _openAddEvent;
		set => SetProperty(ref _openAddEvent, value);
	}

	public bool CanSave
	{
		get => _canSave;
		private set => SetProperty(ref _canSave, value);
	}

	public string NewEventName
	{
		get => _newEventName;

		set
		{
			if (SetProperty(ref _newEventName, value)) UpdateCanSave();
		}
	}

	public DateTime NewDue
	{
		get => _newDue;

		set
		{
			if (SetProperty(ref _newDue, value)) UpdateCanSave();
		}
	}

	public string NewDescription
	{
		get => _newDescription;

		set
		{
			if (SetProperty(ref _newDescription, value)) UpdateCanSave();
		}
	}

	public string AgendaName
	{
		get => _agendaName;

		set
		{
			if (SetProperty(ref _agendaName, value)) UpdateCanSave();
		}
	}

	public RepeatsOption Repeat
	{
		get => _repeat;

		set
		{
			if (SetProperty(ref _repeat, value)) UpdateCanSave();
		}
	}

	public Event? SelectedEvent
	{
		get => _selectedEvent;

		private set
		{
			if (_selectedEvent == value) return;

			_selectedEvent = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(HasRelatedTodos));
			UpdateTodosForSelectedEvent();
		}
	}

	public ObservableCollection<Todo?> TodosForSelectedEvent
	{
		get => _todosForSelectedEvent;
		set => SetProperty(ref _todosForSelectedEvent, value);
	}

	private bool _hasRelatedTodos;
	public bool HasRelatedTodos
	{
		get => _hasRelatedTodos;
		set => SetProperty(ref _hasRelatedTodos, value);
	}

	public IEnumerable<EventsByAgenda> EventsByAgendaName =>
			AgendaNames.Select(name => new EventsByAgenda
				{
					AgendaName = name,
					Events = new ObservableCollection<Event>(
						Events.Where(e => e.AgendaName == name)
					)
				}
			);

	public TodoWindowViewModel? TodoWindowVm { get; }

	public Action? OnEventAddedOrUpdated { get; set; }

	private Color? _newColor;

	public Color? NewColor
	{
		get => _newColor;

		set
		{
			if (SetProperty(ref _newColor, value)) UpdateCanSave();
		}
	}

	#endregion


	#region Commands

	public ICommand AddEventCommand   { get; }
	public ICommand CancelCommand     { get; }
	public ICommand OpenPopupCommand  { get; }
	public ICommand ClosePopupCommand { get; }

	#endregion


	#region Public Methods

	private static Color? StringToColor(string? colorStr)
	{
		if (string.IsNullOrWhiteSpace(colorStr)) return null;

		return Color.TryParse(colorStr, out var color) ? color : null;
	}

	public void LoadEvent(Event? ev)
	{
		_currentEvent  = ev;
		NewEventName   = ev?.Name ?? string.Empty;
		NewDescription = ev?.Description ?? string.Empty;
		NewDue         = ev?.Due ?? DateTime.Today;

		Repeat = RepeatOptions.FirstOrDefault(r => r.Repeats == ev!.Repeats)
		         ?? RepeatOptions[0];

		AgendaName    = ev?.AgendaName ?? string.Empty;
		SelectedEvent = _currentEvent;
		NewColor      = StringToColor(ev?.Color);

		UpdateCanSave();
	}

	public void UpdateCanSave()
	{
		if (TodoWindowVm is null) return;

		bool taskListChanged;

		if (_currentEvent is not null)
		{
			var currentTodos  = _currentEvent.Todos;
			var selectedTodos = TodosForSelectedEvent;

			taskListChanged = !currentTodos.SequenceEqual(selectedTodos);
		}
		else { taskListChanged = TodosForSelectedEvent.Count > 0; }

		var hasTaskChanges =
				!string.IsNullOrWhiteSpace(TodoWindowVm?.NewTaskName.Trim())
				|| !string.IsNullOrWhiteSpace(TodoWindowVm?.SelectedTodoName.Trim())
				|| taskListChanged;

		CanSave = !string.IsNullOrWhiteSpace(NewEventName)
		          && (_currentEvent is null
		              || NewEventName != _currentEvent.Name
		              || NewDescription != _currentEvent.Description
		              || NewDue.Date != _currentEvent.Due.Date
		              || Repeat.Repeats != _currentEvent.Repeats
		              || AgendaName != _currentEvent.AgendaName
		              || hasTaskChanges
		              || NewColor != StringToColor(_currentEvent.Color));

		((RelayCommand)AddEventCommand).NotifyCanExecuteChanged();
	}


	private void AddOrUpdateEvent()
	{
		if (!CanSave) return;

		if (_currentEvent is null)
		{
			var newEvent = new Event(Events.Count + 1, NewEventName)
			{
				Description = NewDescription,
				Due         = NewDue,
				Repeats     = Repeat.Repeats,
				AgendaName  = AgendaName,
				Todos       = [],
				Color       = NewColor?.ToString() ?? "#FFFFFF"
			};

			AddTodosToEvent(newEvent);
			Events.Add(newEvent);
		}
		else
		{
			UpdateEventProperties(_currentEvent);
			AddTodosToEvent(_currentEvent);
		}

		ClearEventForm();
		OnEventAddedOrUpdated?.Invoke();
		UpdateCanSave();
	}

	private void ClearEventForm()
	{
		NewEventName   = string.Empty;
		NewDescription = string.Empty;
		NewDue         = DateTime.Today;
		Repeat         = new RepeatsOption { Repeats = Repeats.None };
		AgendaName     = string.Empty;

		if (TodoWindowVm is not null)
			TodoWindowVm.SelectedTodoName = string.Empty;

		_currentEvent = null;
		SelectedEvent = null;
	}

	public void NotifyTodosForSelectedEventChanged()
	{
		OnPropertyChanged(nameof(TodosForSelectedEvent));
	}

	public void RemoveTodoFromEvent(Todo? todo)
	{
		if (todo is null) return;

		TodosForSelectedEvent.Remove(todo);

		TodoWindowVm?.FreeTodos.Add(todo);

		todo.Event      = null;
		HasRelatedTodos = TodosForSelectedEvent.Count > 0;

		OnPropertyChanged(nameof(TodosForSelectedEvent));
		UpdateCanSave();
	}

	#endregion


	#region Private Helpers

	private void CancelAction()
	{
		OpenAddEvent = false;
		ClearEventForm();
		TodoWindowVm?.ClearTodoForm();
		UpdateCanSave();
	}

	private void UpdateTodosForSelectedEvent()
	{
		TodosForSelectedEvent = new ObservableCollection<Todo?>(SelectedEvent?.Todos ?? []);

		HasRelatedTodos = TodosForSelectedEvent.Count > 0;

		TodosForSelectedEvent.CollectionChanged += (_, _) =>
		{
			HasRelatedTodos = TodosForSelectedEvent.Count > 0;
		};
	}

	private void AddTodosToEvent(Event ev)
	{
		ev.Todos = ev.Todos.Where(t => TodosForSelectedEvent.Contains(t)).ToList();

		foreach (var todo in TodosForSelectedEvent)
		{
			if (ev.Todos.Contains(todo)) continue;

			ev.Todos.Add(todo);

			if (todo is not null) todo.Event = ev;
		}
	}


	private void UpdateEventProperties(Event ev)
	{
		ev.Name        = NewEventName;
		ev.Description = NewDescription;
		ev.Due         = NewDue;
		ev.Repeats     = Repeat.Repeats;
		ev.AgendaName  = AgendaName;
		ev.Color       = NewColor?.ToString() ?? "#FFFFFF";
	}

	private static IEnumerable<Event> GenerateSampleEvents() =>
	[
		new(1, "Conferência da Pamonha")
		{
			Description = "Conferência de pamonhas",
			Due = new DateTime(
				2025,
				4,
				6,
				8,
				0,
				0
			),
			Repeats    = Repeats.Anually,
			AgendaName = "Conferências",
			Color      = "#4CABE4"
		},
		new(2, "Feira da Foda")
		{
			Description = "Conferência do Agro",
			Due = new DateTime(
				2025,
				4,
				5,
				14,
				0,
				0
			),
			Repeats    = Repeats.None,
			AgendaName = "Conferências",
			Color      = "#FFB900"
		},
		new(3, "Festa do Peão")
		{
			Description = "Festa do Peão de Barretos",
			Due = new DateTime(
				2025,
				8,
				20,
				12,
				0,
				0
			),
			Repeats    = Repeats.Monthly,
			AgendaName = "Festas",
			Color      = "#E4080A"
		},
		new(4, "Feriado")
		{
			Description = "Feriado de alguma coisa",
			Due = new DateTime(
				2025,
				4,
				18,
				22,
				0,
				0
			),
			Repeats    = Repeats.Monthly,
			AgendaName = "Feriados",
			Color      = "#7DDA58"
		}
	];

	#endregion
}
