using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Agendai.Data;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase
{
	#region View-Model State

	private string   _newTaskName = string.Empty;
	private bool     _hasChanges;
	private string   _newDescription = string.Empty;
	private DateTime _newDue;
	private string   _listName = "Minhas Tarefas";
	private Todo?    _editingTodo;
	private Todo?    _selectedTodo;
	private string   _selectedTodoName = string.Empty;
	private bool     _suppressPropertyChanged;

	private RepeatsOption               _selectedRepeats = new() { Repeats = Repeats.None };
	private ObservableCollection<Todo?> _freeTodos       = [];
	private ObservableCollection<Todo>  _todos           = [];


	private bool _isPopupOpen;
	private bool _openAddTask;

	private ObservableCollection<string> _listNames       = [];
	private ObservableCollection<Todo>   _todoHistory     = [];
	private ObservableCollection<Todo>   _incompleteTodos = [];
	private ObservableCollection<Todo>?  _incompleteResume;

	#endregion


	public TodoWindowViewModel(HomeWindowViewModel? homeWindowVm = null)
	{
		InitializeCommands();
		InitializeSampleTodos();
		InitializeCollections();

		_newDue = DateTime.Today;

		if (homeWindowVm is not null)
		{
			HomeWindowVm = homeWindowVm;
			EventListVm  = HomeWindowVm.EventListVm;
		}

		PropertyChanged += (_, e) =>
		{
			if (_suppressPropertyChanged) return;

			if (e.PropertyName is not (nameof(NewTaskName)
			                           or nameof(NewDescription)
			                           or nameof(NewDue)
			                           or nameof(SelectedRepeats)
			                           or nameof(ListName)
			                           or nameof(EditingTodo))) return;

			UpdateHasChanges();
			(AddTodoCommand as RelayCommand)?.NotifyCanExecuteChanged();
		};

		RefreshFreeTodos();
	}


	#region Dependencies

	public HomeWindowViewModel? HomeWindowVm { get; set; }
	public EventListViewModel?  EventListVm  { get; set; }
	public Action?              OnTaskAdded  { get; set; }

	#endregion


	#region Commands

	public ICommand? OpenPopupCommand    { get; private set; }
	public ICommand? SelectTarefaCommand { get; private set; }
	public ICommand? AddTodoCommand      { get; private set; }
	public ICommand? CancelCommand       { get; private set; }

	public ICommand? AddTodoToEventCommand { get; private set; }

	private void InitializeCommands()
	{
		OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);

		SelectTarefaCommand = new RelayCommand(() =>
			{
				OpenAddTask = true;
				IsPopupOpen = false;
			}
		);

		AddTodoCommand = new RelayCommand<Event?>(AddTodo, _ => HasChanges);

		CancelCommand = new RelayCommand(() =>
			{
				OpenAddTask = false;
				IsPopupOpen = false;
				ClearTodoForm();
			}
		);

		AddTodoToEventCommand = new RelayCommand(() =>
			{
				var relatedEvent = EventListVm?.SelectedEvent;

				var todo = new Todo(Todos.Count + 1, NewTaskName)
				{
					Description = NewDescription,
					Due         = NewDue,
					Repeats     = SelectedRepeats.Repeats,
					ListName    = ListName,
					Event       = relatedEvent
				};

				todo.OnStatusChanged += HandleStatusChanged;

				Todos.Add(todo);

				RefreshFreeTodos();
				ClearTodoForm();


				if (HomeWindowVm is not null)
				{
					HomeWindowVm.EventListVm.TodosForSelectedEvent.Add(todo);
					HomeWindowVm.EventListVm.HasRelatedTodos =
							HomeWindowVm.EventListVm.TodosForSelectedEvent.Count > 0;

					HomeWindowVm.EventListVm.UpdateCanSave();

					HomeWindowVm.EventListVm.IsAddTodoPopupOpen = false;
				}

				FreeTodos.Remove(todo);
			}
		);
	}

	#endregion


	#region State Tracking

	public bool OpenAddTask
	{
		get => _openAddTask;
		set => SetProperty(ref _openAddTask, value);
	}


	public ObservableCollection<Todo> Todos
	{
		get => _todos;
		set => SetProperty(ref _todos, value);
	}


	public ObservableCollection<Todo> IncompleteTodos
	{
		get => _incompleteTodos;
		set => SetProperty(ref _incompleteTodos, value);
	}

	public ObservableCollection<Todo> TodoHistory
	{
		get => _todoHistory;
		set => SetProperty(ref _todoHistory, value);
	}
	public ObservableCollection<Todo> IncompleteResume
	{
		get => _incompleteResume ?? [];
		set => SetProperty(ref _incompleteResume, value);
	}

	public ObservableCollection<string> ListNames
	{
		get => _listNames;
		set => SetProperty(ref _listNames, value);
	}

	public IEnumerable<TodosByListName> TodosByListName =>
			ListNames.Select(name => new TodosByListName
				{
					ListName = name,
					Items = new ObservableCollection<Todo>(
						Todos.Where(t => t.ListName == name && t.Status == TodoStatus.Incomplete
						)
					)
				}
			);


	public string NewTaskName
	{
		get => _newTaskName;
		set => SetProperty(ref _newTaskName, value);
	}

	public string NewDescription
	{
		get => _newDescription;
		set => SetProperty(ref _newDescription, value);
	}

	public DateTime NewDue
	{
		get => _newDue;
		set => SetProperty(ref _newDue, value);
	}

	public bool IsPopupOpen
	{
		get => _isPopupOpen;
		set => SetProperty(ref _isPopupOpen, value);
	}
	public RepeatsOption SelectedRepeats
	{
		get => _selectedRepeats;
		set => SetProperty(ref _selectedRepeats, value);
	}

	public string ListName
	{
		get => _listName;
		set => SetProperty(ref _listName, value);
	}

	public ObservableCollection<RepeatsOption> RepeatOptions { get; } =
	[
		new() { Repeats = Repeats.None }, new() { Repeats   = Repeats.Daily },
		new() { Repeats = Repeats.Weekly }, new() { Repeats = Repeats.Monthly },
		new() { Repeats = Repeats.Anually }
	];

	public Todo? EditingTodo
	{
		get => _editingTodo;

		set
		{
			if (!SetProperty(ref _editingTodo, value) || value == null) return;

			_suppressPropertyChanged = true;

			NewTaskName    = value.Name;
			NewDescription = value.Description ?? string.Empty;
			NewDue         = value.Due;
			SelectedRepeats = RepeatOptions.FirstOrDefault(r => r.Repeats == value.Repeats)
			                  ?? RepeatOptions[0];
			ListName = value.ListName ?? string.Empty;

			_suppressPropertyChanged = false;
			UpdateHasChanges();
			(AddTodoCommand as RelayCommand)?.NotifyCanExecuteChanged();
		}
	}

	public bool HasChanges
	{
		get => _hasChanges;
		private set => SetProperty(ref _hasChanges, value);
	}

	public ObservableCollection<Todo?> FreeTodos
	{
		get => _freeTodos;

		private set
		{
			if (!SetProperty(ref _freeTodos, value)) return;

			_freeTodos.CollectionChanged += (_, _) =>
			{
				OnPropertyChanged(nameof(FreeTodosNames));
			};

			OnPropertyChanged(nameof(FreeTodosNames));
		}
	}

	public IEnumerable<string> FreeTodosNames =>
			FreeTodos.Select(t => t?.Name)!;
	public Todo? SelectedTodo
	{
		get => _selectedTodo;
		set => SetProperty(ref _selectedTodo, value);
	}

	public string SelectedTodoName
	{
		get => _selectedTodoName;

		set
		{
			if (_selectedTodoName == value) return;

			_selectedTodoName = value;
			OnPropertyChanged();

			var todoName = _selectedTodoName;

			var todo = FreeTodos.FirstOrDefault(t => t?.Name == todoName);

			if (todo is null) return;

			Dispatcher.UIThread.Post(() =>
				{
					FreeTodos.Remove(todo);
					HomeWindowVm?.EventListVm.TodosForSelectedEvent.Add(todo);
					HomeWindowVm?.EventListVm.NotifyTodosForSelectedEventChanged();
					HomeWindowVm?.EventListVm.UpdateCanSave();
				}
			);
		}
	}

	#endregion


	#region Event Handlers

	private void UpdateHasChanges()
	{
		HasChanges = EditingTodo is null
				? !string.IsNullOrWhiteSpace(NewTaskName)
				: EditingTodo.Name != NewTaskName
				  || EditingTodo.Description != NewDescription
				  || EditingTodo.Due != NewDue
				  || EditingTodo.Repeats != SelectedRepeats.Repeats
				  || EditingTodo.ListName != ListName;

		((RelayCommand<Event?>)AddTodoCommand!).NotifyCanExecuteChanged();
	}

	public void ClearTodoForm()
	{
		NewTaskName      = string.Empty;
		NewDescription   = string.Empty;
		NewDue           = DateTime.Today;
		SelectedRepeats  = new RepeatsOption { Repeats = Repeats.None };
		ListName         = string.Empty;
		EditingTodo      = null;
		SelectedTodo     = null;
		SelectedTodoName = string.Empty;
		HasChanges       = false;
	}


	private void RefreshFreeTodos()
	{
		FreeTodos = new ObservableCollection<Todo?>(
			Todos.Where(t => t.Event == null)
		);
	}


	private void AddTodo(Event? relatedEv)
	{
		if (string.IsNullOrWhiteSpace(NewTaskName)) return;

		if (EditingTodo != null)
		{
			EditingTodo.Name        = NewTaskName;
			EditingTodo.Description = NewDescription;
			EditingTodo.Due         = NewDue;
			EditingTodo.Repeats     = SelectedRepeats.Repeats;
			EditingTodo.ListName    = ListName;
			EditingTodo.Event       = relatedEv;

			OnPropertyChanged(nameof(Todos));
			OnPropertyChanged(nameof(TodosByListName));
		}
		else
		{
			Todo newTodo = new(Todos.Count + 1, NewTaskName)
			{
				Description = NewDescription,
				Due         = NewDue,
				Repeats     = SelectedRepeats.Repeats,
				ListName    = ListName,
				Event       = relatedEv
			};

			newTodo.OnStatusChanged += HandleStatusChanged;
			Todos.Add(newTodo);
		}

		RefreshFreeTodos();
		ClearTodoForm();

		IncompleteTodos = new ObservableCollection<Todo>(Todos.Where(t => !Todo.IsComplete(t)));
		IncompleteResume = new ObservableCollection<Todo>(IncompleteTodos.Take(7));

		OnTaskAdded?.Invoke();
	}

	private void HandleStatusChanged(Todo todo, TodoStatus newStatus)
	{
		if (newStatus == TodoStatus.Complete)
		{
			IncompleteTodos.Remove(todo);
			TodoHistory.Add(todo);
		}
		else
		{
			TodoHistory.Remove(todo);
			IncompleteTodos.Add(todo);
		}

		IncompleteResume = new ObservableCollection<Todo>(IncompleteTodos.Take(7));

		ListNames = new ObservableCollection<string>(
			Todos.Select(t => t.ListName).OfType<string>().Distinct()
		);

		OnPropertyChanged(nameof(TodosByListName));
	}

	#endregion


	#region Utils

	private void InitializeSampleTodos()
	{
		_todos = [..Todo.Sample()];

		foreach (var todo in _todos) { todo.OnStatusChanged += HandleStatusChanged; }
	}

	private void InitializeCollections()
	{
		_incompleteTodos =
				new ObservableCollection<Todo>(Todos.Where(t => !Todo.IsComplete(t)));
		_todoHistory = new ObservableCollection<Todo>(Todos.Where(Todo.IsComplete));
		_listNames =
				new ObservableCollection<string>(Todos.Select(t => t.ListName).Distinct()!);
		_incompleteResume = new ObservableCollection<Todo>(_incompleteTodos.Take(7));
	}

	#endregion
}
