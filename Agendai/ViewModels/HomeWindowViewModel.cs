using System.Windows.Input;
using Agendai.Data.Repositories.Interfaces;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
	private readonly ITodoRepository _todoRepository;

	public HomeWindowViewModel(ITodoRepository todoRepository)
	{
        _todoRepository = todoRepository;
    }
	
    private bool _isPopupOpen;

	public bool IsPopupOpen
	{
		get => _isPopupOpen;
		set => SetProperty(ref _isPopupOpen, value);
	}

	public ICommand OpenPopupCommand =>
			new RelayCommand(() => IsPopupOpen = true);

	public ICommand OpenAgendaCommand => new RelayCommand(OpenAgenda);

	public ICommand OpenTodoCommand => new RelayCommand(OpenTodo);

	public ICommand OpenPomodoroCommand => new RelayCommand(OpenPomodoro);
	
	private void OpenAgenda() { MainViewModel?.NavigateToAgenda(); }

	private void OpenTodo() { MainViewModel?.NavigateToTodo(); }

	private void OpenPomodoro() { MainViewModel?.NavigateToPomodoro(); }
}
