using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Agendai.Data;
using Agendai.Models;
using DynamicData.Binding;


namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
	public string Title { get; set; } = "Tarefas";

	private bool _isPopupOpen;
	public bool IsPopupOpen
	{
		get => _isPopupOpen;

		set
		{
			_isPopupOpen = value;
			OnPropertyChanged(nameof(IsPopupOpen));
		}
	}

	private bool _openAddTask;

	public bool OpenAddTask
	{
		get => _openAddTask;

		set
		{
			_openAddTask = value;
			OnPropertyChanged(nameof(OpenAddTask));
		}
	}
	
	public ICommand OpenPopupCommand    { get; }
	public ICommand SelectTarefaCommand { get; }


	public TodoWindowViewModel()
	{
		OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);
		OnTaskAdded      = () => { OpenAddTask = false; };
		SelectTarefaCommand = new RelayCommand(
			() =>
			{
				OpenAddTask = true;
				IsPopupOpen = false;
			}
		);
		AddTodoCommand = new RelayCommand(AddTodo); 

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
				Repeats     = Repeats.None,
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

		_incompleteTodos =
				new ObservableCollection<Todo>(
					Todos.Where(t => !IsComplete(t))
				);
		_todoHistory = new ObservableCollection<Todo>(Todos.Where(IsComplete));

		_listNames =
				new ObservableCollection<string>(
					Todos.Select(t => t.ListName)!
				);
	}

	private Action?  OnTaskAdded    { get; set; }
	public  ICommand AddTodoCommand { get; }
	public ObservableCollection<Repeats> RepeatOptions { get; } =
	[
		Repeats.None, Repeats.Daily, Repeats.Weekly, Repeats.Monthly,
		Repeats.Anually
	];

	private ObservableCollection<Todo>? _todos;
	public ObservableCollection<Todo> Todos
	{
		get => _todos ?? [];

		set
		{
			_todos = value;
			OnPropertyChanged(nameof(Todos));
		}
	}

	private ObservableCollection<Todo>? _incompleteTodos;

	public ObservableCollection<Todo> IncompleteTodos
	{
		get => _incompleteTodos ?? [];

		set
		{
			_incompleteTodos = value;
			OnPropertyChanged(nameof(IncompleteTodos));
		}
	}


	private ObservableCollection<Todo>? _todoHistory;
	public ObservableCollection<Todo> TodoHistory
	{
		get => _todoHistory ?? [];

		set
		{
			_todoHistory = value;
			OnPropertyChanged(nameof(TodoHistory));
		}
	}

	private ObservableCollection<string> _listNames;

	public ObservableCollection<string> ListNames
	{
		get => _listNames;

		set
		{
			_listNames = value;
			OnPropertyChanged(nameof(ListNames));
		}
	}

	private string _newTaskName;
	public string NewTaskName
	{
		get => _newTaskName;

		set
		{
			_newTaskName = value;
			OnPropertyChanged(nameof(NewTaskName));
		}
	}

	private DateTime _newDue;

	public DateTime NewDue
	{
		get => _newDue;

		set
		{
			_newDue = value;
			OnPropertyChanged(nameof(NewDue));
		}
	}

	private string _newDescription;
	public string NewDescription
	{
		get => _newDescription;

		set
		{
			_newDescription = value;
			OnPropertyChanged(NewDescription);
		}
	}
	private Repeats _repeat = Repeats.None;
	public Repeats Repeat
	{
		get => _repeat;

		set
		{
			_repeat = value;
			OnPropertyChanged(nameof(Repeat));
		}
	}

	private string _listName = "Minhas Tarefas";
	public string ListName
	{
		get => _listName;

		set
		{
			_listName = value;
			OnPropertyChanged(nameof(ListName));
		}
	}

	public IEnumerable<TodosByListName> TodosByListName
	{
		get
		{
			return ListNames.Select(
				name => new TodosByListName
				{
					ListName = name,
					Items = Todos.Where(
						t => t.ListName == name
						     && t.Status == TodoStatus.Incomplete
					)
				}
			);
		}
	}

	private bool IsComplete(Todo todo)
	{
		return todo.Status == TodoStatus.Complete;
	}

	public void AddTodo()
	{
		if (string.IsNullOrWhiteSpace(NewTaskName)) return;

		var newTodo = new Todo(Convert.ToUInt32(Todos.Count + 1), NewTaskName)
		{
			Description = NewDescription,
			Due         = NewDue,
			Repeats     = Repeat,
			ListName    = ListName
		};

		Todos.Add(newTodo);

		NewTaskName    = string.Empty;
		NewDescription = string.Empty;
		NewDue         = DateTime.Today;
		Repeat         = Repeats.None;
		ListName       = string.Empty;

		IncompleteTodos = new ObservableCollection<Todo>(
			Todos.Where(t => !IsComplete(t)).ToList()
		);

		OnTaskAdded?.Invoke();
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected new virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(
			this,
			new PropertyChangedEventArgs(propertyName)
		);
	}
}
