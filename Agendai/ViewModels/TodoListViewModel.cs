using System;
using System.Collections.ObjectModel;
using System.Linq;
using Agendai.Models;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class TodoListViewModel : INotifyPropertyChanged
{
	public Action?  OnTaskAdded    { get; set; }
	public ICommand AddTodoCommand { get; }
	public ObservableCollection<Repeats> RepeatOptions { get; } =
	[
		Repeats.None, Repeats.Daily, Repeats.Weekly, Repeats.Monthly,
		Repeats.Anually
	];

	public TodoListViewModel()
	{
		Todos =
		[
			new Todo(1, "Comprar Café")
			{
				Description = "Preciso comprar café",
				Due         = new DateTime(2025, 03, 31),
				Repeats     = Repeats.Monthly,
				Status      = TodoStatus.Complete,
				ListName    = "Compras"
			},
			new Todo(2, "Comprar Pão")
			{
				Description = "Preciso comprar pão",
				Due         = new DateTime(2025, 03, 31),
				Repeats     = Repeats.Weekly,
				ListName    = "Compras"
			},
			new Todo(3, "Comprar Leite")
			{
				Description = "Preciso comprar leite",
				Due         = new DateTime(2025, 03, 31),
				Repeats     = Repeats.Daily,
				ListName    = "Compras"
			},
			new Todo(4, "Treinar Perna")
			{
				Description = "Treinar perna",
				Due         = new DateTime(2025, 03, 31),
				Repeats     = Repeats.Weekly,
				ListName    = "Treino",
				Status      = TodoStatus.Complete
			}
		];

		_incompleteTodos = new ObservableCollection<Todo>(
			Todos.Where(t => !IsComplete(t))
		);

		_todoHistory = new ObservableCollection<Todo>(Todos.Where(IsComplete));

		AddTodoCommand = new RelayCommand(AddTodo);
	}

	private ObservableCollection<Todo> _todos;
	public ObservableCollection<Todo> Todos
	{
		get => _todos;

		set
		{
			_todos = value;
			OnPropertyChanged(nameof(Todos));
		}
	}

	private ObservableCollection<Todo> _incompleteTodos;

	public ObservableCollection<Todo> IncompleteTodos
	{
		get => _incompleteTodos;

		set
		{
			_incompleteTodos = value;
			OnPropertyChanged(nameof(IncompleteTodos));
		}
	}


	private ObservableCollection<Todo> _todoHistory;
	public ObservableCollection<Todo> TodoHistory
	{
		get => _todoHistory;

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
			OnPropertyChanged(ListName);
		}
	}

	public bool IsComplete(Todo todo)
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
			_todos.Where(t => !IsComplete(t)).ToList()
		);

		OnTaskAdded?.Invoke();
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(
			this,
			new PropertyChangedEventArgs(propertyName)
		);
	}
}
