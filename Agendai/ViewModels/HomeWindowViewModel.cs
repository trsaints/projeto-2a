using System;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
	private bool _isPopupOpen;
	private bool _isAgendaWindow;
	private bool _isTodoWindow;
	private bool _isPomodoroWindow;

	public HomeWindowViewModel()
	{
		TodoWindowVm          = new TodoWindowViewModel();
		EventListVm           = new EventListViewModel(TodoWindowVm);
	}

	public TodoWindowViewModel TodoWindowVm { get; set; }
	public EventListViewModel EventListVm { get; set; }

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


	public ICommand OpenPopupCommand => new RelayCommand(() => IsPopupOpen = true);
	public ICommand OpenTodoFormCommand => new RelayCommand(OpenTodoForm);
	public ICommand OpenEventFormCommand => new RelayCommand(OpenEventForm);
	public ICommand OpenAgendaCommand => new RelayCommand(OpenAgenda);
	public ICommand OpenTodoCommand => new RelayCommand(OpenTodo);
	public ICommand OpenPomodoroCommand => new RelayCommand(OpenPomodoro);

	private void OpenAgenda() { MainViewModel?.NavigateToAgenda(); }

	private void OpenTodo() { MainViewModel?.NavigateToTodo(); }

	private void OpenPomodoro() { MainViewModel?.NavigateToPomodoro(); }

	private void OpenTodoForm()
	{
		IsPopupOpen = false;
		if (TodoWindowVm != null)
		{
			TodoWindowVm.OpenAddTask = true;
		}
	}

	private void OpenEventForm()
	{
		IsPopupOpen = false;
		if (EventListVm != null)
		{
			EventListVm.OpenAddEvent = true;
			
		}
	}
	
	private string[] _selectedListNames = Array.Empty<string>();

	public string[] SelectedListNames
	{
		get => _selectedListNames;
		set => SetProperty(ref _selectedListNames, value);
	}
	
	public void AddSelectedListName(string listName)
	{
		if (!_selectedListNames.Contains(listName))
		{
			SelectedListNames = _selectedListNames.Concat(new[] { listName }).ToArray();
		}
	}

	public void RemoveSelectedListName(string listName)
	{
		if (_selectedListNames.Contains(listName))
		{
			SelectedListNames = _selectedListNames.Where(name => name != listName).ToArray();
		}
	}
}
