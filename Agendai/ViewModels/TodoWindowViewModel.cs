using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Agendai.Data;
using Agendai.Data.Converters;
using Agendai.Data.Models;
using Agendai.Messages;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;
using CommunityToolkit.Mvvm.Messaging;


namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase
{
	public TodoWindowViewModel(HomeWindowViewModel? homeWindowVm = null)
	{
		InitializeCommands();
		InitializeSampleTodos();
		InitializeCollections();

		_newDue = DateTime.Today;

		if (homeWindowVm is not null)
		{
			HomeWindowVm = homeWindowVm;
			EventListVm  = homeWindowVm.EventListVm;
		}

		WeakReferenceMessenger.Default.Register<GetListsNamesMessenger>(
			this,
			(r, msg) =>
			{
				ListasSelecionadas = msg.SelectedItemsName.ToHashSet();
			}
		);

		PropertyChanged += (_, e) =>
		{
			if (_suppressPropertyChanged) return;

			if (e.PropertyName is not (nameof(NewTaskName)
			                           or nameof(NewDescription)
			                           or nameof(NewDue)
			                           or nameof(SelectedRepeats)
			                           or nameof(ListName)
			                           or nameof(EditingTodo)))
			{
				return;
			}
			
			UpdateHasChanges();
			(AddTodoCommand as RelayCommand)?.NotifyCanExecuteChanged();
		};

		if (ListasSelecionadas is not null || ListasSelecionadas!.Count is 0)
		{
			ListasSelecionadas = [..ListNames];
		}

		RefreshFreeTodos();
	}


	#region Dependências

	public HomeWindowViewModel? HomeWindowVm { get; set; }
	public EventListViewModel   EventListVm  { get; set; }
	public Action?              OnTaskAdded  { get; set; }

	#endregion


	#region Comandos

	public ICommand OpenPopupCommand         { get; private set; }
	public ICommand SelectTarefaCommand      { get; private set; }
	public ICommand AddTodoCommand           { get; private set; }
	public ICommand CancelCommand            { get; private set; }
	public ICommand DeleteTodoCommand        { get; private set; }
	public ICommand SkippedTodoCommand       { get; private set; }
	public ICommand SortMinhasTarefasCommand { get; private set; }
	public ICommand SortHistoricoCommand     { get; private set; }
	public ICommand SortListCommand          { get; private set; }
	public ICommand ConfirmDeleteCommand     { get; private set; }
	public ICommand CancelDeleteCommand      { get; private set; }


	private void InitializeCommands()
	{
		OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);

		SelectTarefaCommand = new RelayCommand<Todo>(
			(todo) =>
			{
				EditingTodo = todo;
				OpenAddTask = true;
				IsPopupOpen = false;
			}
		);

		AddTodoCommand = new RelayCommand(
			() => AddTodo(null),
			() => HasChanges
		);


		CancelCommand = new RelayCommand(
			() =>
			{
				OpenAddTask = false;
				IsPopupOpen = false;
				ClearTodoForm();
			}
		);

		DeleteTodoCommand = new RelayCommand<Todo>(
			RequestDeleteOrSkipTodo,
			todo => todo != null
		);
		SkippedTodoCommand = new RelayCommand<Todo>(
			SkippedTodo,
			todo => todo != null
		);

		SortMinhasTarefasCommand = new RelayCommand<string>(
			sort =>
			{
				if (string.IsNullOrEmpty(sort)) return;

				SortMinhasTarefas = SortTypeValue(sort);
			}
		);

		SortHistoricoCommand = new RelayCommand<string>(
			sort =>
			{
				if (string.IsNullOrEmpty(sort)) return;

				SortHistorico = SortTypeValue(sort);
			}
		);

		SortListCommand = new RelayCommand<object[]>(
			paramsArray =>
			{
				if (paramsArray is not { Length: 2 }) return;

				var listName = paramsArray[0].ToString();
				var sort     = paramsArray[1].ToString();

				if (string.IsNullOrEmpty(listName)
				    || string.IsNullOrEmpty(sort)) { return; }

				// Atualiza o tipo de ordenação para a lista específica
				var newSortType = SortTypeValue(sort);
				_listSortTypes[listName] = newSortType;

				// Reordena explicitamente os itens da lista correspondente
				var itemsQuery  = Todos.Where(t => t.ListName == listName);
				var sortedItems = SortTodos(itemsQuery, newSortType);

				// Atualiza a coleção correspondente
				var listToUpdate = TodosByListName.FirstOrDefault(l => l.ListName == listName);
				if (listToUpdate != null)
				{
					listToUpdate.Items = new ObservableCollection<Todo>(sortedItems);
				}

				// Notifica a interface do usuário sobre a mudança
				OnPropertyChanged(nameof(TodosByListName));
			}
		);

		CancelDeleteCommand = new RelayCommand(
			() => { IsDeleteConfirmationVisible = false; }
		);

		ConfirmDeleteCommand = new RelayCommand(
			() =>
			{
				var todoToDelete = TodoForDeletion;

				if (todoToDelete == null) return;

				todoToDelete.OnStatusChanged -= HandleStatusChanged;

				IsDeleteConfirmationVisible = false;

				Todos.Remove(todoToDelete);
				IncompleteTodos.Remove(todoToDelete);
				TodoHistory.Remove(todoToDelete);

				var listNameExists =
						Todos.Any(t => t.ListName == todoToDelete.ListName);

				if (!listNameExists)
				{
					ListNames.Remove(todoToDelete.ListName);
				}

				RefreshFreeTodos();

				OnPropertyChanged(nameof(TodosByListName));
			}
		);
	}

	private static SortType SortTypeValue(string sort)
	{
		return sort switch
		{
			"Nome"      => SortType.Nome,
			"Prazo"     => SortType.Prazo,
			"NomeLista" => SortType.NomeLista,
			_           => SortType.Nome
		};
	}

	#endregion


	#region Propriedades de UI

	private bool _isDeleteConfirmationVisible;
	public bool IsDeleteConfirmationVisible
	{
		get => _isDeleteConfirmationVisible;
		set => SetProperty(ref _isDeleteConfirmationVisible, value);
	}

	private Todo? _todoForDeletion;
	public Todo? TodoForDeletion
	{
		get => _todoForDeletion;

		set
		{
			_todoForDeletion = value;
			OnPropertyChanged();
		}
	}

	private bool _isPopupOpen;
	public bool IsPopupOpen
	{
		set => SetProperty(ref _isPopupOpen, value);
	}

	private bool _openAddTask;
	public bool OpenAddTask
	{
		get => _openAddTask;

		set
		{
			if (!SetProperty(ref _openAddTask, value) || !value) return;

			if (EditingTodo is not null) return;

			SelectedRepeats = RepeatOptions.FirstOrDefault()
			                  ?? new RepeatsOption { Repeats = Repeats.None };
			ListName = ListNames.FirstOrDefault() ?? string.Empty;
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

	private ObservableCollection<string?> _listNames = [];
	public ObservableCollection<string?> ListNames
	{
		get => _listNames;
		set => SetProperty(ref _listNames, value);
	}

	private readonly Dictionary<string, SortType> _listSortTypes = new();

	private HashSet<string?> _listasSelecionadas = [];

	private HashSet<string?> ListasSelecionadas
	{
		get => _listasSelecionadas;

		set
		{
			_listasSelecionadas = value;
			OnPropertyChanged(nameof(TodosByListName));
			OnPropertyChanged(nameof(TodosFiltrados));
		}
	}

	public IEnumerable<TodosByListName> TodosByListName
	{
		get
		{
			return ListNames
			       .Where(name => ListasSelecionadas.Contains(name))
			       .Select(
				       name =>
				       {
					       var sortType =
							       _listSortTypes.TryGetValue(
								       name!,
								       out var type
							       )
									       ? type
									       : SortType.Prazo;

					       var itemsQuery =
							       Todos.Where(t => t.ListName == name);

					       var sortedItems =
							       sortType switch
							       {
								       SortType.Nome => itemsQuery.OrderBy(
									       t => t.Name
								       ),
								       SortType.NomeLista => itemsQuery.OrderBy(
									       t => t.ListName
								       ),
								       _ => itemsQuery.OrderBy(t => t.Due),
							       };

					       return new TodosByListName
					       {
						       ListName = name,
						       Items = new ObservableCollection<Todo>(
							       sortedItems
						       )
					       };
				       }
			       );
		}
	}


	public IEnumerable<Todo> TodosFiltrados =>
			TodosByListName.SelectMany(g => g.Items);

	#endregion


	#region Nova Tarefa (Formulário)

	private Event? _selectedEvent;
	public Event? SelectedEvent
	{
		get => _selectedEvent;
		set => SetProperty(ref _selectedEvent, value);
	}

	private string _newTaskName;
	public string NewTaskName
	{
		get => _newTaskName;
		set => SetProperty(ref _newTaskName, value);
	}

	private string? _newDescription;
	public string? NewDescription
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

	private string? _listName;
	public string? ListName
	{
		get => _listName;
		set => SetProperty(ref _listName, value);
	}

	public ObservableCollection<RepeatsOption> RepeatOptions { get; } =
	[
		new() { Repeats = Repeats.None }, new() { Repeats = Repeats.Daily },
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

				NewTaskName    = value.Name;
				NewDescription = value.Description;
				NewDue         = value.Due;
				SelectedRepeats =
						RepeatOptions.FirstOrDefault(
							r => r.Repeats == value.Repeats
						)
						?? RepeatOptions[0];
				ListName      = value.ListName;
				SelectedEvent = value.RelatedEvent;

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
		NewTaskName      = string.Empty;
		NewDescription   = string.Empty;
		NewDue           = DateTime.Today;
		SelectedRepeats  = new RepeatsOption { Repeats = Repeats.None };
		ListName         = string.Empty;
		EditingTodo      = null;
		SelectedTodo     = null;
		SelectedTodoName = null;
		HasChanges       = false;
	}

	#endregion


	#region Seleção de Tarefa Existente

	private ObservableCollection<Todo> _freeTodos = [];
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

	private string? _selectedTodoName;
	public string? SelectedTodoName
	{
		get => _selectedTodoName;

		set
		{
			if (_selectedTodoName == value) return;

			_selectedTodoName = value;
			OnPropertyChanged();
			SelectedTodo = FreeTodos.FirstOrDefault(t => t.Name == value);
		}
	}

	private void RefreshFreeTodos()
	{
		FreeTodos = new ObservableCollection<Todo>(
			Todos.Where(t => t.RelatedEvent == null)
		);
	}

	public ObservableCollection<string> SortOptions { get; } =
	[
		"Nome", "Prazo", "NomeLista"
	];

	private SortType _sortMinhasTarefas = SortType.Prazo;
	public SortType SortMinhasTarefas
	{
		get => _sortMinhasTarefas;

		set
		{
			_sortMinhasTarefas = value;
			OrdenarMinhasTarefas();
		}
	}

	private static IEnumerable<Todo> SortTodos(
		IEnumerable<Todo> todos,
		SortType          sortType
	)
	{
		return sortType switch
		{
			SortType.Nome      => todos.OrderBy(t => t.Name),
			SortType.Prazo     => todos.OrderBy(t => t.Due),
			SortType.NomeLista => todos.OrderBy(t => t.ListName),
			_                  => todos
		};
	}

	private void OrdenarMinhasTarefas()
	{
		var incomplete = Todos.Where(t => !IsComplete(t));
		var ordenado   = SortTodos(incomplete, SortMinhasTarefas);
		IncompleteTodos = new ObservableCollection<Todo>(ordenado);
		OnPropertyChanged(nameof(IncompleteTodos));
	}

	private SortType _sortHistorico = SortType.Prazo;
	public SortType SortHistorico
	{
		get => _sortHistorico;

		set
		{
			_sortHistorico = value;
			OrdenarHistorico();
		}
	}

	private void OrdenarHistorico()
	{
		var complete = Todos.Where(IsComplete);
		var ordenado = SortTodos(complete, SortHistorico);
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
			EditingTodo.Name         = NewTaskName;
			EditingTodo.Description  = NewDescription;
			EditingTodo.Due          = NewDue;
			EditingTodo.Repeats      = SelectedRepeats.Repeats;
			EditingTodo.ListName     = ListName;
			EditingTodo.RelatedEvent = relatedEv;
			todo                     = EditingTodo;

			OnPropertyChanged(nameof(Todos));
			OnPropertyChanged(nameof(TodosByListName));
		}
		else
		{
			uint newId = 1;

			if (Todos.Any()) { newId = (uint)(Todos.Max(t => t.Id) + 1); }

			todo = new Todo(newId, NewTaskName)
			{
				Description  = NewDescription,
				Due          = NewDue,
				Repeats      = SelectedRepeats.Repeats,
				ListName     = ListName,
				RelatedEvent = relatedEv
			};

			todo.OnStatusChanged += HandleStatusChanged;
			Todos.Add(todo);

			if (!ListNames.Contains(todo.ListName))
			{
				ListNames.Add(todo.ListName);
				OnPropertyChanged(nameof(ListNames));
			}

			if (ListasSelecionadas.Add(todo.ListName))
			{
				OnPropertyChanged(nameof(TodosByListName));
			}
		}

		RefreshFreeTodos();
		ClearTodoForm();

		IncompleteTodos =
				new ObservableCollection<Todo>(
					Todos.Where(t => !IsComplete(t))
				);
		IncompleteResume =
				new ObservableCollection<Todo>(IncompleteTodos.Take(7));

		OnPropertyChanged(nameof(IncompleteTodos));
		OnPropertyChanged(nameof(IncompleteResume));
		OnPropertyChanged(nameof(TodosByListName));
		OnPropertyChanged(nameof(ListNames));

		OrdenarMinhasTarefas();

		OpenAddTask = false;
		OnPropertyChanged(nameof(OpenAddTask));

		OnTaskAdded?.Invoke();

		return todo;
	}


	private void RequestDeleteOrSkipTodo(Todo? todo)
	{
		if (todo == null) return;

		if (todo.Status == TodoStatus.Incomplete)
		{
			todo.Status = TodoStatus.Skipped;
		}
		else
		{
			TodoForDeletion             = todo;
			IsDeleteConfirmationVisible = true;
		}
	}

	private static void SkippedTodo(Todo? todo)
	{
		if (todo == null) return;

		todo.Status = TodoStatus.Skipped;
	}


	private void HandleStatusChanged(Todo todo, TodoStatus newStatus)
	{
		if (newStatus is TodoStatus.Complete or TodoStatus.Skipped)
		{
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

		OrdenarHistorico();
		OrdenarMinhasTarefas();

		IncompleteResume =
				new ObservableCollection<Todo>(IncompleteTodos.Take(7));

		ListNames = new ObservableCollection<string?>(
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
	}

	private void InitializeCollections()
	{
		_incompleteTodos =
				new ObservableCollection<Todo>(
					Todos.Where(t => !IsComplete(t))
				);
		_todoHistory = new ObservableCollection<Todo>(
			Todos.Where(
				t => t.Status is TodoStatus.Complete or TodoStatus.Skipped
			)
		);
		_listNames =
				new ObservableCollection<string?>(
					Todos.Select(t => t.ListName).Distinct()
				);
		_incompleteResume =
				new ObservableCollection<Todo>(_incompleteTodos.Take(7));
	}

	#endregion


	private bool _suppressPropertyChanged;

	private static bool IsComplete(Todo todo) =>
			todo.Status is TodoStatus.Complete or TodoStatus.Skipped;
}
