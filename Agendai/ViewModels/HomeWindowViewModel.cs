using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;
using Agendai.Utils;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
    private readonly ITodoRepository _todoRepository;
    private readonly ObservableCollection<Todo> _todos = [];

    public HomeWindowViewModel(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
        DatabaseUtils.MapToObservable(_todoRepository, _todos).Wait();
    }

    public ObservableCollection<Todo> RecentTodos => [
        .. _todos.Where(t => t.Status == TodoStatus.Incomplete).Take(10)
    ];

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
