using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class ViewModelBase : ObservableObject
{
	protected bool                          _isPopupOpen;
	protected bool                          _isAgendaWindow;
	protected bool                          _isTodoWindow;
	protected bool                          _isPomodoroWindow;
	protected bool                          _openAddEvent;
	private   string?                       _listName = string.Empty;
	private   bool                          _isAddTodoPopupOpen;
	protected ObservableCollection<string?> _listNames = [];


	public MainWindowViewModel? MainViewModel { get; set; }
	public TodoWindowViewModel? TodoWindowVm { get; set; }
	public EventListViewModel? EventListVm { get; set; }
	public ICommand ReturnHomeCommand => new RelayCommand(ReturnHome);


	#region Commands

	public ICommand OpenPopupCommand =>
			new RelayCommand(() => IsPopupOpen = true);
	public ICommand OpenTodoFormCommand  => new RelayCommand(OpenTodoForm);
	public ICommand OpenEventFormCommand => new RelayCommand(OpenEventForm);
	public ICommand OpenAgendaCommand    => new RelayCommand(OpenAgenda);
	public ICommand OpenTodoCommand      => new RelayCommand(OpenTodo);
	public ICommand OpenPomodoroCommand  => new RelayCommand(OpenPomodoro);
	public ICommand ClosePopupCommand =>
			new RelayCommand(() => IsAddTodoPopupOpen = false);

	#endregion


	private void ReturnHome() { MainViewModel?.NavigateToHome(); }

	private void OpenAgenda() { MainViewModel?.NavigateToAgenda(); }

	private void OpenTodo() { MainViewModel?.NavigateToTodo(); }

	private void OpenPomodoro() { MainViewModel?.NavigateToPomodoro(); }

	private void OpenTodoForm()
	{
		IsPopupOpen = false;

		if (TodoWindowVm is not null) { TodoWindowVm.OpenAddTask = true; }
	}

	private void OpenEventForm()
	{
		IsPopupOpen = false;

		if (EventListVm is not null) { OpenAddEvent = true; }
	}

	public bool IsPopupOpen
	{
		get => _isPopupOpen;
		set => SetProperty(ref _isPopupOpen, value);
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
		get => _isPomodoroWindow;
		set => SetProperty(ref _isPomodoroWindow, value);
	}

	public ObservableCollection<string?> ListNames
	{
		get => _listNames;
		set => SetProperty(ref _listNames, value);
	}

	public bool OpenAddEvent
	{
		get => _openAddEvent;
		set => SetProperty(ref _openAddEvent, value);
	}

	public string? ListName
	{
		get => _listName;
		set => SetProperty(ref _listName, value);
	}


	public bool IsAddTodoPopupOpen
	{
		get => _isAddTodoPopupOpen;
		set => SetProperty(ref _isAddTodoPopupOpen, value);
	}
}
