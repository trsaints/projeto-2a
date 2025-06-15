using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Agendai.Data;
using Agendai.Data.Converters;
using Agendai.Models;
using DynamicData.Binding;


namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase
{
	public TodoWindowViewModel()
	{
		OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);
		OnTaskAdded      = () => { OpenAddTask = false; };
		SelectTarefaCommand = new RelayCommand<Todo>(
			(todo) =>
			{
				SelectedRepeats = RepeatOptions.First();
				ListName = ListNames.FirstOrDefault() ?? string.Empty;
				OpenAddTask = true;
				IsPopupOpen = false;
				
				SelectedTodo = todo;
			}
		);
		AddTodoCommand = new RelayCommand(AddTodo);
		CancelCommand = new RelayCommand(
			() =>
			{
				OpenAddTask = false;
				IsPopupOpen = false;
			}
		);

		_todos =
		[
			new Todo(1, "Comprar Pamonha")
			{
				Description = "Comprar pamonha na feira",
				Due         = DateTime.Today,
				Repeats     = Repeats.None,
				ListName    = "Compras",
				Status      = TodoStatus.Complete
			},
			new Todo(2, "Treino Fullbody")
			{
				Description = "Treino fullbody na feira",
				Due         = DateTime.Today,
				Repeats     = Repeats.Daily,
				ListName    = "Treinos"
			},
			new Todo(3, "Lavar o chão")
			{
				Description = "Lavar o chão da sala",
				Due         = DateTime.Today,
				Repeats     = Repeats.None,
				ListName    = "Casa"
			},
			new Todo(4, "Lavar o banheiro")
			{
				Description = "Lavar o banheiro",
				Due         = DateTime.Today,
				Repeats     = Repeats.None,
				ListName    = "Casa",
				Status      = TodoStatus.Complete
			}
		];

		foreach (var todo in _todos)
		{
			todo.OnStatusChanged += HandleStatusChanged;
		}

		_incompleteTodos =
				new ObservableCollection<Todo>(
					Todos.Where(t => !IsComplete(t))
				);
		_todoHistory = new ObservableCollection<Todo>(Todos.Where(IsComplete));

		_listNames = new ObservableCollection<string>(
			Todos
					.Select(t => t.ListName)
					.OfType<string>()
					.Distinct()
		);

		_incompleteResume =
				new ObservableCollection<Todo>(_incompleteTodos.Take(7));
		
	}

	public string Title { get; set; } = "Tarefas";
	
	private Todo? _selectedTodo;
    public Todo? SelectedTodo
    {
        get => _selectedTodo;
        set => SetProperty(ref _selectedTodo, value);
    }
	
    private bool _isEditPanelOpen;
    public bool IsEditPanelOpen
    {
	    get => _isEditPanelOpen;
	    set => SetProperty(ref _isEditPanelOpen, value);
    }
    

	private bool _isPopupOpen;
	public bool IsPopupOpen
	{
		get => _isPopupOpen;

		set => SetProperty(ref _isPopupOpen, value);
	}

	private bool _openAddTask;

	public bool OpenAddTask
	{
		get => _openAddTask;

		set => SetProperty(ref _openAddTask, value);
	}

	public ICommand OpenPopupCommand    { get; }
	public ICommand SelectTarefaCommand { get; }


	public RepeatsConverter RepeatsConverter { get; } = new();

	private Action?  OnTaskAdded    { get; set; }
	public  ICommand AddTodoCommand { get; }

	public ICommand CancelCommand { get; }

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

		IncompleteResume = new ObservableCollection<Todo>(
			IncompleteTodos.Take(7)
		);

		ListNames = new ObservableCollection<string>(
			Todos.Select(t => t.ListName).OfType<string>().Distinct()
		);

		OnPropertyChanged(nameof(TodosByListName));
	}

	private ObservableCollection<Todo>? _todos;
	public ObservableCollection<Todo> Todos
	{
		get => _todos ?? [];

		set => SetProperty(ref _todos, value);
	}

	private ObservableCollection<Todo>? _incompleteTodos;

	public ObservableCollection<Todo> IncompleteTodos
	{
		get => _incompleteTodos ?? [];

		set => SetProperty(ref _incompleteTodos, value);
	}

	private ObservableCollection<Todo>? _incompleteResume;

	public ObservableCollection<Todo> IncompleteResume
	{
		get => _incompleteResume ?? [];

		set => SetProperty(ref _incompleteResume, value);
	}

	private ObservableCollection<Todo>? _todoHistory;
	public ObservableCollection<Todo> TodoHistory
	{
		get => _todoHistory ?? [];

		set => SetProperty(ref _todoHistory, value);
	}

	private ObservableCollection<string> _listNames;

	public ObservableCollection<string> ListNames
	{
		get => _listNames;

		set => SetProperty(ref _listNames, value);
	}

	private string _newTaskName;
	public string NewTaskName
	{
		get => _newTaskName;

		set => SetProperty(ref _newTaskName, value);
	}

	private DateTime _newDue;

	public DateTime NewDue
	{
		get => _newDue;

		set => SetProperty(ref _newDue, value);
	}

	private string _newDescription;
	public string NewDescription
	{
		get => _newDescription;

		set => SetProperty(ref _newDescription, value);
	}

	private RepeatsOption _selectedRepeats; 
	public RepeatsOption SelectedRepeats
	{
		get => _selectedRepeats;

		set => SetProperty(ref _selectedRepeats, value);
	}

	private string _listName = "";
	public string ListName
	{
		get => _listName;

		set => SetProperty(ref _listName, value);
	}

	public IEnumerable<TodosByListName> TodosByListName
	{
		get
		{
			return ListNames.Select(
				name => new TodosByListName
				{
					ListName = name,
					Items = new ObservableCollection<Todo>(
						Todos.Where(
							t => t.ListName == name
							     && t.Status == TodoStatus.Incomplete
						)
					)
				}
			);
		}
	}

	public ObservableCollection<RepeatsOption> RepeatOptions { get; } =
	[
		new() { Repeats = Repeats.None }, new() { Repeats = Repeats.Daily },
		new() { Repeats = Repeats.Weekly },
		new() { Repeats = Repeats.Monthly },
		new() { Repeats = Repeats.Anually }
	];

	private bool IsComplete(Todo todo)
	{
		return todo.Status == TodoStatus.Complete;
	}

	public void AddTodo()
	{
		if (string.IsNullOrWhiteSpace(NewTaskName)) return;
		if (string.IsNullOrWhiteSpace(ListName)) return;

		var newTodo = new Todo(Convert.ToUInt32(Todos.Count + 1), NewTaskName)
		{
			Description = NewDescription,
			Due = NewDue,
			Repeats = RepeatsConverter.ConvertBack(SelectedRepeats.ToString()),
			ListName = ListName
		};

		newTodo.OnStatusChanged += HandleStatusChanged;

		Todos.Add(newTodo);

		NewTaskName    = string.Empty;
		NewDescription = string.Empty;
		NewDue         = DateTime.Today;
		SelectedRepeats = new RepeatsOption
		{
			Repeats = Repeats.None
		};
		ListName = string.Empty;

		IncompleteTodos = new ObservableCollection<Todo>( 
			Todos.Where(t => !IsComplete(t)).ToList()
		);

		IncompleteResume =
				new ObservableCollection<Todo>(IncompleteTodos.Take(7));
		ListNames = new ObservableCollection<string>(
			Todos.Select(t => t.ListName).OfType<string>().Distinct()
		);
		
		OnPropertyChanged(nameof(TodosByListName));

		OnTaskAdded?.Invoke();
	}
}
