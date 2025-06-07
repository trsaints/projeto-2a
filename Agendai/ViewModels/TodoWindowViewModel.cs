using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Agendai.Data.Converters;
using Agendai.Data.Models;
using Agendai.Data.Dtos;
using Agendai.Data.Repositories.Interfaces;


namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase
{
    private bool _isPopupOpen;
    private bool _openAddTask;
    private Action? OnTaskAdded { get; set; }
    private ObservableCollection<Todo>? _todos;
    private ObservableCollection<Todo>? _incompleteTodos;
    private ObservableCollection<Todo>? _todoHistory;
    private ObservableCollection<Todo>? _incompleteResume;
    private ObservableCollection<string> _listNames;
    private string _newTaskName = string.Empty;
    private DateTime? _newDue;
    private string _newDescription = string.Empty;
    private RepeatsOption? _selectedRepeats;
    private string _listName = "Minhas Tarefas";
    private readonly ITodoRepository _todoRepository;

    public TodoWindowViewModel(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;

        OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);
        OnTaskAdded = () => { OpenAddTask = false; };
        SelectTarefaCommand = new RelayCommand(
            () =>
            {
                OpenAddTask = true;
                IsPopupOpen = false;
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

        _todos = [];

        foreach (var todo in _todos)
        {
            todo.OnStatusChanged += HandleStatusChanged;
        }

        _incompleteTodos = [.. Todos.Where(t => !IsComplete(t))];
        _todoHistory = [.. Todos.Where(IsComplete)];

        _listNames = [.. Todos.Select(t => t.ListName).OfType<string>().Distinct()];

        _incompleteResume = [.. _incompleteTodos.Take(7)];
    }

    public string Title { get; set; } = "Tarefas";

    public bool IsPopupOpen
    {
        get => _isPopupOpen;

        set => SetProperty(ref _isPopupOpen, value);
    }


    public bool OpenAddTask
    {
        get => _openAddTask;

        set => SetProperty(ref _openAddTask, value);
    }

    public ICommand OpenPopupCommand { get; }
    public ICommand SelectTarefaCommand { get; }


    public RepeatsConverter RepeatsConverter { get; } = new();

    public ICommand AddTodoCommand { get; }

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

    public ObservableCollection<Todo> Todos
    {
        get => _todos ?? [];

        set => SetProperty(ref _todos, value);
    }


    public ObservableCollection<Todo> IncompleteTodos
    {
        get => _incompleteTodos ?? [];

        set => SetProperty(ref _incompleteTodos, value);
    }


    public ObservableCollection<Todo> IncompleteResume
    {
        get => _incompleteResume ?? [];

        set => SetProperty(ref _incompleteResume, value);
    }

    public ObservableCollection<Todo> TodoHistory
    {
        get => _todoHistory ?? [];

        set => SetProperty(ref _todoHistory, value);
    }


    public ObservableCollection<string> ListNames
    {
        get => _listNames;

        set => SetProperty(ref _listNames, value);
    }

    public string NewTaskName
    {
        get => _newTaskName;

        set => SetProperty(ref _newTaskName, value);
    }

    public DateTime NewDue
    {
        get => _newDue ?? DateTime.Now;

        set => SetProperty(ref _newDue, value);
    }

    public string NewDescription
    {
        get => _newDescription;

        set => SetProperty(ref _newDescription, value);
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

        var newTodo = new Todo(Convert.ToUInt32(Todos.Count + 1), NewTaskName)
        {
            Description = NewDescription,
            Due = NewDue,
            Repeats = RepeatsConverter.ConvertBack(SelectedRepeats.ToString()),
            ListName = ListName
        };

        newTodo.OnStatusChanged += HandleStatusChanged;

        Todos.Add(newTodo);

        NewTaskName = string.Empty;
        NewDescription = string.Empty;
        NewDue = DateTime.Today;
        SelectedRepeats = new RepeatsOption
        {
            Repeats = Repeats.None
        };
        ListName = string.Empty;

        IncompleteTodos = [.. Todos.Where(t => !IsComplete(t)).ToList()];

        IncompleteResume = [.. IncompleteTodos.Take(7)];

        OnTaskAdded?.Invoke();
    }
}
