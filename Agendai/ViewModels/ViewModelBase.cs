using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels;

public class ViewModelBase : ObservableObject
{
    private bool _isPopupOpen;
    private bool _isAgendaWindow;
    private bool _isTodoWindow;
    private bool _isPomodoroWindow;

    public MainWindowViewModel? MainViewModel { get; set; }
    public TodoWindowViewModel? TodoWindowVm { get; set; }
    public EventListViewModel? EventListVm { get; set; }
    public ICommand ReturnHomeCommand => new RelayCommand(ReturnHome);

    public ICommand OpenPopupCommand => new RelayCommand(() => IsPopupOpen = true);
    public ICommand OpenTodoFormCommand => new RelayCommand(OpenTodoForm);
    public ICommand OpenEventFormCommand => new RelayCommand(OpenEventForm);
    public ICommand OpenAgendaCommand => new RelayCommand(OpenAgenda);
    public ICommand OpenTodoCommand => new RelayCommand(OpenTodo);
    public ICommand OpenPomodoroCommand => new RelayCommand(OpenPomodoro);

    private void ReturnHome()
    {
        MainViewModel?.NavigateToHome();
    }

    private void OpenAgenda() { MainViewModel?.NavigateToAgenda(); }

    private void OpenTodo() { MainViewModel?.NavigateToTodo(); }

    private void OpenPomodoro() { MainViewModel?.NavigateToPomodoro(); }

    private void OpenTodoForm()
    {
        IsPopupOpen = false;

        if (TodoWindowVm is not null)
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
}
