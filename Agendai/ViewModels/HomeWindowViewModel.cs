using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
    private ICommand? _openAgendaCommand;
    public ICommand OpenAgendaCommand => _openAgendaCommand ??= new RelayCommand(OpenAgenda);

    private ICommand? _openTodoCommand;
    public ICommand OpenTodoCommand => _openTodoCommand ??= new RelayCommand(OpenTodo);

    private ICommand? _openPomodoroCommand;
    public ICommand OpenPomodoroCommand => _openPomodoroCommand ??= new RelayCommand(OpenPomodoro);


    private void OpenAgenda()
    {
        MainViewModel?.NavigateToAgenda();
    }

    private void OpenTodo()
    {
        MainViewModel?.NavigateToTodo();
    }

    private void OpenPomodoro()
    {
        MainViewModel?.NavigateToPomodoro();
    }
}
