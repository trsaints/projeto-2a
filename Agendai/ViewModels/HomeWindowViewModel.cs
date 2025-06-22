using System.Windows.Input;
using Agendai.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase, IHomeWindowViewModel
{
	#region View-Model State

	private bool _isPopupOpen;
	private bool _isEventListsAbleToView = true;
	private bool _isTodoListsAbleToView  = true;


	private bool _isAgendaWindow;
	private bool _isTodoWindow;
	private bool _isPomodoroWindow;

	#endregion


	#region Dependencies

	public TodoWindowViewModel? TodoWindowVm { get; set; }
	public EventListViewModel?  EventListVm  { get; set; }

	#endregion


	public HomeWindowViewModel()
	{
		OpenPopupCommand     = new RelayCommand(() => IsPopupOpen = true);
		OpenTodoFormCommand  = new RelayCommand(OpenTodoForm);
		OpenAgendaCommand    = new RelayCommand(OpenAgenda);
		OpenTodoCommand      = new RelayCommand(OpenTodo);
		OpenPomodoroCommand  = new RelayCommand(OpenPomodoro);
		OpenEventFormCommand = new RelayCommand(OpenEventForm);
		TodoWindowVm         = new TodoWindowViewModel(this);
		EventListVm          = new EventListViewModel(TodoWindowVm);
	}

	public string EventListsVisibilityText => IsEventListsAbleToView ? "Ocultar" : "Exibir";
	public string TodoListsVisibilityText  => IsTodoListsAbleToView ? "Ocultar" : "Exibir";


	#region Commands

	public ICommand OpenPopupCommand     { get; }
	public ICommand OpenTodoFormCommand  { get; }
	public ICommand OpenEventFormCommand { get; }
	public ICommand OpenAgendaCommand    { get; }
	public ICommand OpenTodoCommand      { get; }
	public ICommand OpenPomodoroCommand  { get; }

	#endregion


	#region State Tracking

	public bool IsPopupOpen
	{
		get => _isPopupOpen;
		set => SetProperty(ref _isPopupOpen, value);
	}

	public bool IsEventListsAbleToView
	{
		get => _isEventListsAbleToView;

		set
		{
			if (SetProperty(ref _isEventListsAbleToView, value))
			{
				OnPropertyChanged(nameof(EventListsVisibilityText));
			}
		}
	}

	public bool IsTodoListsAbleToView
	{
		get => _isTodoListsAbleToView;

		set
		{
			if (SetProperty(ref _isTodoListsAbleToView, value))
			{
				OnPropertyChanged(nameof(TodoListsVisibilityText));
			}
		}
	}
	public bool IsAgendaWindow
	{
		get => _isAgendaWindow;
		set => SetProperty(ref _isAgendaWindow, value);
	}

	public bool IsTodoWindow
	{
		get => _isTodoWindow;
		set => SetProperty(ref _isTodoWindow, value);
	}

	public bool IsPomodoroWindow
	{
		set => SetProperty(ref _isPomodoroWindow, value);
	}

	#endregion


	#region Event Handlers

	private void OpenAgenda() { MainViewModel?.NavigateToAgenda(); }

	private void OpenTodo() { MainViewModel?.NavigateToTodo(); }

	private void OpenPomodoro() { MainViewModel?.NavigateToPomodoro(); }

	private void OpenTodoForm()
	{
		IsPopupOpen = false;

		if (TodoWindowVm is null) return;

		TodoWindowVm.OpenAddTask = true;
	}

	private void OpenEventForm()
	{
		IsPopupOpen = false;

		if (EventListVm is null) return;

		EventListVm.OpenAddEvent = true;
	}

	#endregion
}
