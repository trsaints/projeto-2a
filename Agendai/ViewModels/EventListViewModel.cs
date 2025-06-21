using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using Agendai.Data;
using Agendai.Data.Models;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;


namespace Agendai.ViewModels
{
	public class EventListViewModel : ViewModelBase
	{
		#region Fields

		private Event?                       _currentEvent;
		private Event?                       _selectedEvent;
		private bool                         _canSave;
		private string                       _newEventName = string.Empty;
		private DateTime                     _newDue = DateTime.Today;
		private string                       _newDescription = string.Empty;
		private string                       _agendaName = string.Empty;
		private Repeats                      _repeat = Repeats.None;
		private ObservableCollection<Todo>   _todosForSelectedEvent = [];
		private ObservableCollection<string> _agendaNames;

		#endregion


		#region Constructor

		public EventListViewModel(TodoWindowViewModel? todoWindowVm = null)
		{
			TodoWindowVm = todoWindowVm;
					
			SelectTarefaCommand = new RelayCommand(() => OpenAddEvent = true);
			AddEventCommand = new RelayCommand(AddOrUpdateEvent, () => CanSave);
			CancelCommand = new RelayCommand(CancelAction);

			OnEventAddedOrUpdated = () => { OpenAddEvent = false; };

			Events = [..Event.Samples()];
			_agendaNames = new ObservableCollection<string>(
				Events.Select(e => e.AgendaName).Distinct()!
			);

			TodoWindowVm.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName is nameof(TodoWindowVm.NewTaskName)
				                      or nameof(TodoWindowVm.SelectedTodoName))
				{
					UpdateCanSave();
				}
			};
		}

		#endregion


		#region Properties

		public ObservableCollection<Event> Events { get; set; }

		public ObservableCollection<Repeats> RepeatOptions { get; } =
		[
			Repeats.None, Repeats.Daily, Repeats.Weekly, Repeats.Monthly,
			Repeats.Anually
		];

		public ObservableCollection<string> AgendaNames
		{
			get => _agendaNames;
			set => SetProperty(ref _agendaNames, value);
		}

		public  ICommand AddEventCommand     { get; }
		public  ICommand CancelCommand       { get; }
		public  ICommand SelectTarefaCommand { get; }


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

		public Repeats Repeat
		{
			get => _repeat;

			set
			{
				if (SetProperty(ref _repeat, value)) UpdateCanSave();
			}
		}

		private Event? SelectedEvent
		{
			get => _selectedEvent;

			set
			{
				if (_selectedEvent == value) return;

				_selectedEvent = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasRelatedTodos));
				UpdateTodosForSelectedEvent();
			}
		}

		public ObservableCollection<Todo> TodosForSelectedEvent
		{
			get => _todosForSelectedEvent;
			set => SetProperty(ref _todosForSelectedEvent, value);
		}

		public bool HasRelatedTodos => SelectedEvent?.Todos is { Count: not 0 };

		public IEnumerable<EventsByAgenda> EventsByAgendaName =>
				AgendaNames.Select(
					name => new EventsByAgenda
					{
						AgendaName = name,
						Events = new ObservableCollection<Event>(
							Events.Where(e => e.AgendaName == name)
						)
					}
				);

		public Action? OnEventAddedOrUpdated { get; set; }

		#endregion


		#region Public Methods

		public void LoadEvent(Event? ev)
		{
			_currentEvent  = ev;
			NewEventName   = ev?.Name ?? "";
			NewDescription = ev?.Description ?? "";
			NewDue         = ev?.Due ?? DateTime.Today;
			Repeat         = ev?.Repeats ?? Repeats.None;
			AgendaName     = ev?.AgendaName ?? "";
			SelectedEvent  = _currentEvent;
			UpdateCanSave();
		}

		private void UpdateCanSave()
		{
			if (TodoWindowVm == null) return;

			var hasTaskChanges =
					!string.IsNullOrWhiteSpace(TodoWindowVm.NewTaskName?.Trim())
					|| !string.IsNullOrWhiteSpace(
						TodoWindowVm.SelectedTodoName?.Trim()
					);

			CanSave = !string.IsNullOrWhiteSpace(NewEventName)
			          && (
				          _currentEvent is null
				          || NewEventName != _currentEvent.Name
				          || NewDescription != _currentEvent.Description
				          || NewDue.Date != _currentEvent.Due.Date
				          || Repeat != _currentEvent.Repeats
				          || AgendaName != _currentEvent.AgendaName
				          || hasTaskChanges
			          );

			((RelayCommand)AddEventCommand).NotifyCanExecuteChanged();
		}

		private void AddOrUpdateEvent()
		{
			if (!CanSave) return;

			if (_currentEvent is null)
			{
				var newEvent = new Event((uint)(Events.Count + 1), NewEventName)
				{
					Description = NewDescription,
					Due         = NewDue,
					Repeats     = Repeat,
					AgendaName  = AgendaName,
					Todos       = new List<Todo>()
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
			Repeat         = Repeats.None;
			AgendaName     = string.Empty;

			if (TodoWindowVm != null) TodoWindowVm.SelectedTodoName = null;

			_currentEvent = null;
			SelectedEvent = null;
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
			TodosForSelectedEvent = new ObservableCollection<Todo>(
				SelectedEvent?.Todos ?? Enumerable.Empty<Todo>()
			);
		}

		private void AddTodosToEvent(Event ev)
		{
			if (TodoWindowVm?.SelectedTodo is not null
			    && ev.Todos != null
			    && !ev.Todos.Contains(TodoWindowVm.SelectedTodo))
			{
				TodoWindowVm.AddTodo(ev);
				ev.Todos.Add(TodoWindowVm.SelectedTodo);
			}

			if (string.IsNullOrWhiteSpace(TodoWindowVm?.NewTaskName)) return;
			
			var newTodo = TodoWindowVm.AddTodo(ev);

			if (newTodo != null && ev.Todos != null && !ev.Todos.Contains(newTodo))
			{
				ev.Todos.Add(newTodo);
			}
		}

		private void UpdateEventProperties(Event ev)
		{
			ev.Name        = NewEventName;
			ev.Description = NewDescription;
			ev.Due         = NewDue;
			ev.Repeats     = Repeat;
			ev.AgendaName  = AgendaName;

			ev.Todos ??= new List<Todo>();
		}

		#endregion
	}
}
