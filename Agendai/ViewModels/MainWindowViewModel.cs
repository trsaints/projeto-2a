using System;
using Agendai.ViewModels.Agenda;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agendai.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        // Initialize with HomeViewModel
        CurrentViewModel = new HomeWindowViewModel();
        // Pass reference to main view model after construction
        if (CurrentViewModel is HomeWindowViewModel homeViewModel)
        {
            homeViewModel.MainViewModel = this;
        }
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
        var homeViewModel = new HomeWindowViewModel();
        homeViewModel.MainViewModel = this;
        CurrentViewModel = homeViewModel;
    }

    public void NavigateToAgenda()
    {
        var agendaViewModel = new AgendaWindowViewModel();
        agendaViewModel.MainViewModel = this;
        CurrentViewModel = agendaViewModel;
    }

    public void NavigateToTodo()
    {
        var todoViewModel = new TodoWindowViewModel();
        todoViewModel.MainViewModel = this;
        CurrentViewModel = todoViewModel;
    }

    public void NavigateToPomodoro()
    {
        var pomodoroViewModel = new PomodoroWindowViewModel();
        pomodoroViewModel.MainViewModel = this;
        CurrentViewModel = pomodoroViewModel;
    }
    
    public void NavigateToSpecificDay(DateTime selectedDate)
    {
        if (CurrentViewModel is AgendaWindowViewModel agendaViewModel)
        {
            agendaViewModel.CurrentDay = selectedDate;
            agendaViewModel.SelectedIndex = 2;
            agendaViewModel.UpdateDataGridItems();
        }
        else
        {
            var newAgendaViewModel = new AgendaWindowViewModel();
            newAgendaViewModel.MainViewModel = this;
            newAgendaViewModel.SelectedIndex = 2;
            newAgendaViewModel.UpdateDataGridItems();
        }
    }
}
