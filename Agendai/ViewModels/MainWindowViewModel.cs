using System;
using Agendai.ViewModels.Agenda;

namespace Agendai.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Agendai.Navigators;
using Agendai.Data.Repositories.Interfaces;

public class MainWindowViewModel : ViewModelBase
{
    protected readonly IEventRepository EventRepository;
    protected readonly ITodoRepository TodoRepository;
    protected readonly IShiftRepository ShiftRepository;

    public MainWindowViewModel(IEventRepository eventRepository,
        ITodoRepository todoRepository,
        IShiftRepository shiftRepository)
    {
        EventRepository = eventRepository;
        TodoRepository = todoRepository;
        ShiftRepository = shiftRepository;

        // Initialize with HomeViewModel
        CurrentViewModel = new HomeWindowViewModel(TodoRepository);
        // Pass reference to main view model after construction
        if (CurrentViewModel is HomeWindowViewModel homeViewModel)
        {
            homeViewModel.MainViewModel = this;
        }

        WeakReferenceMessenger.Default.Register<NavigateToDateMessenger>(this, (r, message) =>
        {
            NavigateToSpecificDay(message.SelectedDate);
        });
    }

    private ViewModelBase? _currentViewModel;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    // Navigation methods
    public void NavigateToHome()
    {
        var homeViewModel = new HomeWindowViewModel(TodoRepository)
        {
            MainViewModel = this
        };

        CurrentViewModel = homeViewModel;
    }

    public void NavigateToAgenda()
    {
        var agendaViewModel = new AgendaWindowViewModel(EventRepository, TodoRepository)
        {
            MainViewModel = this
        };
        CurrentViewModel = agendaViewModel;
    }

    public void NavigateToTodo()
    {
        var todoViewModel = new TodoWindowViewModel(TodoRepository)
        {
            MainViewModel = this
        };
        CurrentViewModel = todoViewModel;
    }

    public void NavigateToPomodoro()
    {
        var pomodoroViewModel = new PomodoroWindowViewModel
        {
            MainViewModel = this
        };

        CurrentViewModel = pomodoroViewModel;
    }

    public void NavigateToSpecificDay(DateTime selectedDate)
    {
        switch (CurrentViewModel)
        {
            case AgendaWindowViewModel agendaViewModel:
                {
                    agendaViewModel.CurrentDay = selectedDate;
                    agendaViewModel.SelectedIndex = 2;
                    agendaViewModel.DayController.UpdateDayFromDate(selectedDate);

                    break;
                }

            default:
                {
                    CurrentViewModel = new AgendaWindowViewModel(EventRepository,
                        TodoRepository,
                        selectedDate,
                        2);

                    if (CurrentViewModel is AgendaWindowViewModel agendaViewModel)
                    {
                        agendaViewModel.MainViewModel = this;
                        agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
                    }

                    break;
                }
        }

    }


}
