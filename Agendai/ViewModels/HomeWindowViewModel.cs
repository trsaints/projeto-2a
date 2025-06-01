using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
	private bool _isPopupOpen;

	public TodoWindowViewModel TodoWindowVM { get; set; }
	public EventListViewModel EventListVM { get; set; }

	private ICommand _openPopupCommand;
	private ICommand _openTodoFormCommand;
	private ICommand _openAgendaCommand;
	private ICommand _openTodoCommand;
	private ICommand _openPomodoroCommand;
	private ICommand _openEventFormCommand;

	public bool IsPopupOpen
	{
		get => _isPopupOpen;
		set => SetProperty(ref _isPopupOpen, value);
	}

	public ICommand OpenPopupCommand => _openPopupCommand;
	public ICommand OpenTodoFormCommand => _openTodoFormCommand;
	
	public ICommand OpenEventFormCommand => _openEventFormCommand;
	public ICommand OpenAgendaCommand => _openAgendaCommand;
	public ICommand OpenTodoCommand => _openTodoCommand;
	public ICommand OpenPomodoroCommand => _openPomodoroCommand;

	public HomeWindowViewModel()
	{
		_openPopupCommand = new RelayCommand(() => IsPopupOpen = true);
		_openTodoFormCommand = new RelayCommand(OpenTodoForm);
		_openAgendaCommand = new RelayCommand(OpenAgenda);
		_openTodoCommand = new RelayCommand(OpenTodo);
		_openPomodoroCommand = new RelayCommand(OpenPomodoro);
		_openEventFormCommand = new RelayCommand(OpenEventForm);
		TodoWindowVM = new TodoWindowViewModel();
		EventListVM = new EventListViewModel();
	}

	private void OpenAgenda() { MainViewModel?.NavigateToAgenda(); }

	private void OpenTodo() { MainViewModel?.NavigateToTodo(); }

	private void OpenPomodoro() { MainViewModel?.NavigateToPomodoro(); }

	private void OpenTodoForm()
	{
		IsPopupOpen = false;
		if (TodoWindowVM != null)
		{
			TodoWindowVM.OpenAddTask = true;
		}
	}

	private void OpenEventForm()
	{
		IsPopupOpen = false;
		if (EventListVM != null)
		{
			EventListVM.OpenAddEvent = true;
			
		}
	}
}
