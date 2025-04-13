using System;
using System.Collections.ObjectModel;
using System.Linq;
using Agendai.Models;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    
    public Action? OnTaskAdded { get; set; }
    public ICommand AddTodoCommand { get; }
    public ObservableCollection<Repeats> RepeatOptions { get; } = new ObservableCollection<Repeats>
    {
        Repeats.None,
        Repeats.Daily,
        Repeats.Weekly,
        Repeats.Monthly,
        Repeats.Anually
    };
    
    public TodoListViewModel()
    {
        
        Todos = new ObservableCollection<Todo>
        {
            new Todo(1, "Comprar Café")
            {
                Description = "Preciso comprar café",
                Due = new DateTime(2025, 03, 31),
                Repeats = Repeats.Monthly,
                Status = TodoStatus.Complete
            },
            new Todo(2, "Comprar Pão")
            {
                Description = "Preciso comprar pão",
                Due = new DateTime(2025, 03, 31),
                Repeats = Repeats.Weekly
            },
            new Todo(3, "Comprar Leite")
            {
                Description = "Preciso comprar leite",
                Due = new DateTime(2025, 03, 31),
                Repeats = Repeats.Daily
            }
        };

        CompletedTasksTodo();
        
        AddTodoCommand = new RelayCommand(AddTodo);
    }

    private ObservableCollection<Todo> _todos;
    public ObservableCollection<Todo> Todos
    {
        get => _todos;
        set
        {
            _todos = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<Todo> CompletedTasks { get; set; } = new();

    public ObservableCollection<string> Options { get; } = new ObservableCollection<string>
        {  "Minhas Tarefas", "Faculdade", "Trabalho" };
    
    private string _newTaskName;
    public string NewTaskName
    {
        get => _newTaskName;
        set
        {
            _newTaskName = value;
            OnPropertyChanged();
        }
    }

    private DateTime _newDue;

    public DateTime NewDue
    {
        get => _newDue;
        set
        {
            _newDue = value;
            OnPropertyChanged();
        }
    }

    private string _newDescription;
    public string NewDescription
    {
        get => _newDescription;
        set
        {
            _newDescription = value;
            OnPropertyChanged();
        }
    }
    private Repeats _repeat = Repeats.None;
    public Repeats Repeat
    {
        get => _repeat;
        set
        {
            _repeat = value;
            OnPropertyChanged();
        }
    }

    private string _listName = "Minhas Tarefas";
    public string ListName
    {
        get => _listName;
        set
        {
            _listName = value;
            OnPropertyChanged();
        }
    }

    public bool IsComplete(Todo todo)
    {
        return todo.Status == TodoStatus.Complete;
    }

    private void CompletedTasksTodo()
    {
        var completed = Todos.Where(IsComplete).ToList();
        foreach (var task in completed)
        {
            CompletedTasks.Add(task);
            Todos.Remove(task);
        }
    }

    public void AddTodo()
    {
        if (String.IsNullOrWhiteSpace(NewTaskName)) return;
        
        var newTodo = new Todo(Convert.ToUInt32(Todos.Count + 1), NewTaskName)
        {
            Description = NewDescription,
            Due = NewDue,
            Repeats = Repeat,
            ListName = ListName
        };
        Todos.Add(newTodo);
        
        NewTaskName = string.Empty;
        NewDescription = String.Empty;
        NewDue = DateTime.Today;
        Repeat = Repeats.None;
        ListName = string.Empty;
        
        OnTaskAdded?.Invoke();
    }
}
