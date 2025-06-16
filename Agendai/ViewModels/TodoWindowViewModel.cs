using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Agendai.Data;
using Agendai.Data.Converters;
using Agendai.Data.Models;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase
{
    public TodoWindowViewModel(HomeWindowViewModel homeWindowVm = null)
    {
        InitializeCommands();
        InitializeSampleTodos();
        InitializeCollections();

        _newDue = DateTime.Today;

        if (homeWindowVm != null)
        {
            HomeWindowVm = homeWindowVm;
            EventListVm = homeWindowVm.EventListVm;
        }

        PropertyChanged += (_, e) =>
        {
            if (_suppressPropertyChanged) return;

            if (e.PropertyName is nameof(NewTaskName)
                or nameof(NewDescription)
                or nameof(NewDue)
                or nameof(SelectedRepeats)
                or nameof(ListName)
                or nameof(EditingTodo))
            {
                UpdateHasChanges();
                (AddTodoCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        };

        RefreshFreeTodos();
    }

    #region Dependências
    public HomeWindowViewModel HomeWindowVm { get; set; }
    public EventListViewModel EventListVm { get; set; }
    public RepeatsConverter RepeatsConverter { get; } = new();
    public Action? OnTaskAdded { get; set; }
    #endregion

    #region Comandos
    public ICommand OpenPopupCommand { get; private set; }
    public ICommand SelectTarefaCommand { get; private set; }
    public ICommand AddTodoCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand DeleteTodoCommand { get; private set; }
    public ICommand SkippedTodoCommand { get; private set;}
    public ICommand SortMinhasTarefasCommand { get; private set; }
    public ICommand SortHistoricoCommand { get; private set; }

    private void InitializeCommands()
    {
        OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);

        SelectTarefaCommand = new RelayCommand<Todo>((todo) =>
        {
            EditingTodo = todo;
            OpenAddTask = true;
            IsPopupOpen = false;
        });

        AddTodoCommand = new RelayCommand(
            () => AddTodo(null),
            () => HasChanges
        );


        CancelCommand = new RelayCommand(() =>
        {
            OpenAddTask = false;
            IsPopupOpen = false;
            ClearTodoForm();
        });
        
        DeleteTodoCommand = new RelayCommand<Todo>(
            todo => DeleteTodo(todo),
            todo => todo != null
        );
        SkippedTodoCommand = new RelayCommand<Todo>(
            todo => SkippedTodo(todo),
            todo => todo != null
        );
        
        SortMinhasTarefasCommand = new RelayCommand<string>(sort =>
        {
            if (string.IsNullOrEmpty(sort)) return;

            SortMinhasTarefas = sort switch
            {
                "Nome" => SortType.Nome,
                "Prazo" => SortType.Prazo,
                "NomeLista" => SortType.NomeLista,
                _ => SortType.Nome
            };
        });
        
        SortHistoricoCommand = new RelayCommand<string>(sort =>
        {
            if (string.IsNullOrEmpty(sort)) return;

            SortHistorico = sort switch
            {
                "Nome" => SortType.Nome,
                "Prazo" => SortType.Prazo,
                "NomeLista" => SortType.NomeLista,
                _ => SortType.Nome
            };
        });


    }
    #endregion

    #region Propriedades de UI
    public string Title { get; set; } = "Tarefas";

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
        set
        {
            if (SetProperty(ref _openAddTask, value) && value)
            {
                if (EditingTodo == null)
                {
                    SelectedRepeats = RepeatOptions.FirstOrDefault() ?? new RepeatsOption { Repeats = Repeats.None };
                    ListName = ListNames.FirstOrDefault() ?? string.Empty;
                }
            }
        }
    }
    #endregion

    #region Tarefas e Listagens

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

    private ObservableCollection<Todo>? _todoHistory;
    public ObservableCollection<Todo> TodoHistory
    {
        get => _todoHistory ?? [];
        set => SetProperty(ref _todoHistory, value);
    }

    private ObservableCollection<Todo>? _incompleteResume;
    public ObservableCollection<Todo> IncompleteResume
    {
        get => _incompleteResume ?? [];
        set => SetProperty(ref _incompleteResume, value);
    }

    private ObservableCollection<string> _listNames = [];
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
                Todos.Where(t => t.ListName == name && t.Status != TodoStatus.Skipped)
            )
        });
    #endregion

    #region Nova Tarefa (Formulário)

    private string _newTaskName;
    public string NewTaskName
    {
        get => _newTaskName;
        set => SetProperty(ref _newTaskName, value);
    }

    private string _newDescription;
    public string NewDescription
    {
        get => _newDescription;
        set => SetProperty(ref _newDescription, value);
    }

    private DateTime _newDue;
    public DateTime NewDue
    {
        get => _newDue;
        set => SetProperty(ref _newDue, value);
    }

    private RepeatsOption _selectedRepeats;
    public RepeatsOption SelectedRepeats
    {
        get => _selectedRepeats;
        set => SetProperty(ref _selectedRepeats, value);
    }

    private string _listName;
    public string ListName
    {
        get => _listName;
        set => SetProperty(ref _listName, value);
    }

    public ObservableCollection<RepeatsOption> RepeatOptions { get; } =
    [
        new() { Repeats = Repeats.None },
        new() { Repeats = Repeats.Daily },
        new() { Repeats = Repeats.Weekly },
        new() { Repeats = Repeats.Monthly },
        new() { Repeats = Repeats.Anually }
    ];

    private Todo? _editingTodo;
    public Todo? EditingTodo
    {
        get => _editingTodo;
        set
        {
            if (SetProperty(ref _editingTodo, value) && value != null)
            {
                _suppressPropertyChanged = true;

                NewTaskName = value.Name;
                NewDescription = value.Description;
                NewDue = value.Due;
                SelectedRepeats = RepeatOptions.FirstOrDefault(r => r.Repeats == value.Repeats) ?? RepeatOptions[0];
                ListName = value.ListName;

                _suppressPropertyChanged = false;
                UpdateHasChanges();
                (AddTodoCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    private bool _hasChanges;
    public bool HasChanges
    {
        get => _hasChanges;
        private set
        {
            if (SetProperty(ref _hasChanges, value))
            {
                (AddTodoCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    private void UpdateHasChanges()
    {
        HasChanges = EditingTodo == null
            ? !string.IsNullOrWhiteSpace(NewTaskName)
            : EditingTodo.Name != NewTaskName
              || EditingTodo.Description != NewDescription
              || EditingTodo.Due != NewDue
              || EditingTodo.Repeats != SelectedRepeats.Repeats
              || EditingTodo.ListName != ListName;
    }

    public void ClearTodoForm()
    {
        NewTaskName = string.Empty;
        NewDescription = string.Empty;
        NewDue = DateTime.Today;
        SelectedRepeats = new RepeatsOption { Repeats = Repeats.None };
        ListName = string.Empty;
        EditingTodo = null;
        SelectedTodo = null;
        SelectedTodoName = null;
        HasChanges = false;
    }

    #endregion

    #region Seleção de Tarefa Existente

    private ObservableCollection<Todo> _freeTodos = new();
    public ObservableCollection<Todo> FreeTodos
    {
        get => _freeTodos;
        set => SetProperty(ref _freeTodos, value);
    }

    public IEnumerable<string> FreeTodosNames =>
        FreeTodos.Select(t => t.Name);

    private Todo? _selectedTodo;
    public Todo? SelectedTodo
    {
        get => _selectedTodo;
        set => SetProperty(ref _selectedTodo, value);
    }

    private string _selectedTodoName;
    public string SelectedTodoName
    {
        get => _selectedTodoName;
        set
        {
            if (_selectedTodoName != value)
            {
                _selectedTodoName = value;
                OnPropertyChanged();
                SelectedTodo = FreeTodos.FirstOrDefault(t => t.Name == value);
            }
        }
    }

    private void RefreshFreeTodos()
    {
        FreeTodos = new ObservableCollection<Todo>(
            Todos.Where(t => t.RelatedEvent == null)
        );
    }
    
    public enum SortType
    {
        Nome,
        Prazo,
        NomeLista
    }

    private SortType _sortMinhasTarefas = SortType.Nome;
    public SortType SortMinhasTarefas
    {
        get => _sortMinhasTarefas;
        set
        {
            _sortMinhasTarefas = value;
            OrdenarMinhasTarefas();
            OnPropertyChanged(nameof(SortMinhasTarefas));
        }
    }
    
    private void OrdenarMinhasTarefas()
    {
        var ordenado = SortMinhasTarefas switch
        {
            SortType.Nome => Todos.Where(t => !IsComplete(t)).OrderBy(t => t.Name),
            SortType.Prazo => Todos.Where(t => !IsComplete(t)).OrderBy(t => t.Due),
            SortType.NomeLista => Todos.Where(t => !IsComplete(t)).OrderBy(t => t.ListName),
            _ => Todos.Where(t => !IsComplete(t))
        };

        IncompleteTodos = new ObservableCollection<Todo>(ordenado);
        OnPropertyChanged(nameof(IncompleteTodos));
    }
    
    private SortType _sortHistorico = SortType.Nome;
    public SortType SortHistorico
    {
        get => _sortHistorico;
        set
        {
            _sortHistorico = value;
            OrdenarHistorico();
            OnPropertyChanged(nameof(SortHistorico));
        }
    }
    
    private void OrdenarHistorico()
    {
        var ordenado = SortHistorico switch
        {
            SortType.Nome => Todos.Where(IsComplete).OrderBy(t => t.Name),
            SortType.Prazo => Todos.Where(IsComplete).OrderBy(t => t.Due),
            SortType.NomeLista => Todos.Where(IsComplete).OrderBy(t => t.ListName),
            _ => Todos.Where(IsComplete)
        };

        TodoHistory = new ObservableCollection<Todo>(ordenado);
        OnPropertyChanged(nameof(TodoHistory));
    }


    #endregion

    #region Métodos principais

    public Todo? AddTodo(Event? relatedEv)
    {
        if (string.IsNullOrWhiteSpace(NewTaskName)) return null;
    
        Todo todo;
    
        if (EditingTodo != null)
        {
            EditingTodo.Name = NewTaskName;
            EditingTodo.Description = NewDescription;
            EditingTodo.Due = NewDue;
            EditingTodo.Repeats = SelectedRepeats.Repeats;
            EditingTodo.ListName = ListName;
            EditingTodo.RelatedEvent = relatedEv;
            todo = EditingTodo;
    
            OnPropertyChanged(nameof(Todos));
            OnPropertyChanged(nameof(TodosByListName));
        }
        else
        {
            todo = new Todo(Convert.ToUInt32(Todos.Count + 1), NewTaskName)
            {
                Description = NewDescription,
                Due = NewDue,
                Repeats = SelectedRepeats.Repeats,
                ListName = ListName,
                RelatedEvent = relatedEv
            };
    
            todo.OnStatusChanged += HandleStatusChanged;
            Todos.Add(todo);
        }
    
        RefreshFreeTodos();
        ClearTodoForm();
    
        IncompleteTodos = new ObservableCollection<Todo>(Todos.Where(t => !IsComplete(t)));
        IncompleteResume = new ObservableCollection<Todo>(IncompleteTodos.Take(7));
    
        OnPropertyChanged(nameof(IncompleteTodos));
        OnPropertyChanged(nameof(IncompleteResume));
        OnPropertyChanged(nameof(TodosByListName));
        
        OrdenarMinhasTarefas();
    
        OpenAddTask = false;
        OnPropertyChanged(nameof(OpenAddTask));
    
        OnTaskAdded?.Invoke();
        return todo;
    }


    private void DeleteTodo(Todo todo)
    {
        if (todo == null) return;

        Todos.Remove(todo);
        IncompleteTodos.Remove(todo);
        TodoHistory.Remove(todo);
        RefreshFreeTodos();

        ListNames = new ObservableCollection<string>(Todos.Select(t => t.ListName).Distinct());

        OnPropertyChanged(nameof(TodosByListName));
        OnPropertyChanged(nameof(Todos));
        OnPropertyChanged(nameof(IncompleteResume));

        if (EditingTodo == todo)
        {
            ClearTodoForm();
            OpenAddTask = false;
        }
    }
    
    public void SkippedTodo(Todo todo)
    {
        if (todo == null) return;

        todo.Status = TodoStatus.Skipped;

        IncompleteTodos.Remove(todo);
        if (!TodoHistory.Contains(todo))
            TodoHistory.Add(todo);

        
        IncompleteResume = new ObservableCollection<Todo>(IncompleteTodos.Take(7));
        ListNames = new ObservableCollection<string>(
            Todos.Select(t => t.ListName).Where(n => !string.IsNullOrEmpty(n)).Distinct()
        );

        OnPropertyChanged(nameof(TodosByListName));
        OnPropertyChanged(nameof(IncompleteTodos));
        OnPropertyChanged(nameof(TodoHistory));
        OnPropertyChanged(nameof(IncompleteResume));
        OnPropertyChanged(nameof(ListNames));
    }


    private void HandleStatusChanged(Todo todo, TodoStatus newStatus)
    {
        if (newStatus == TodoStatus.Complete || newStatus == TodoStatus.Skipped)
        {
            if (IncompleteTodos.Contains(todo))
                IncompleteTodos.Remove(todo);

            if (!TodoHistory.Contains(todo))
                TodoHistory.Add(todo);
        }
        else
        {
            if (TodoHistory.Contains(todo))
                TodoHistory.Remove(todo);

            if (!IncompleteTodos.Contains(todo))
                IncompleteTodos.Add(todo);
        }

        IncompleteResume = new ObservableCollection<Todo>(IncompleteTodos.Take(7));

        ListNames = new ObservableCollection<string>(
            Todos.Select(t => t.ListName).OfType<string>().Distinct()
        );

        OnPropertyChanged(nameof(TodosByListName));
    }


    private void InitializeSampleTodos()
    {
        _todos =
        [
            new Todo(1, "Comprar Pamonha")
            {
                Description = "Comprar pamonha na feira",
                Due = DateTime.Today,
                Repeats = Repeats.None,
                ListName = "Compras",
                Status = TodoStatus.Complete
            },
            new Todo(2, "Treino Fullbody")
            {
                Description = "Treino fullbody na feira",
                Due = DateTime.Today,
                Repeats = Repeats.Daily,
                ListName = "Treinos"
            },
            new Todo(3, "Lavar o chão")
            {
                Description = "Lavar o chão da sala",
                Due = DateTime.Today,
                Repeats = Repeats.None,
                ListName = "Casa"
            },
            new Todo(4, "Lavar o banheiro")
            {
                Description = "Lavar o banheiro",
                Due = DateTime.Today,
                Repeats = Repeats.None,
                ListName = "Casa",
                Status = TodoStatus.Complete
            }
        ];

        foreach (var todo in _todos)
        {
            todo.OnStatusChanged += HandleStatusChanged;
        }
    }

    private void InitializeCollections()
    {
        _incompleteTodos = new ObservableCollection<Todo>(Todos.Where(t => !IsComplete(t)));
        _todoHistory = new ObservableCollection<Todo>(Todos.Where(IsComplete));
        _listNames = new ObservableCollection<string>(Todos.Select(t => t.ListName).Distinct());
        _incompleteResume = new ObservableCollection<Todo>(_incompleteTodos.Take(7));
    }

    #endregion

    private bool _suppressPropertyChanged = false;

    private bool IsComplete(Todo todo) => todo.Status == TodoStatus.Complete;
}
