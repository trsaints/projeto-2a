using System;
using Agendai.Models;
using Agendai.ViewModels.Agenda;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Agendai.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Agendai.Messages;

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
        var homeViewModel = new HomeWindowViewModel();
        homeViewModel.MainViewModel = this;
        CurrentViewModel = homeViewModel;
    }

    public void NavigateToAgenda()
    {
        if (CurrentViewModel is HomeWindowViewModel homeViewModel)
        {
            var agendaViewModel = new AgendaWindowViewModel(homeViewModel, null, 0);
            agendaViewModel.MainViewModel = this;
            CurrentViewModel = agendaViewModel;
        }
        else
        {
            var fallbackHomeVm = new HomeWindowViewModel();
            fallbackHomeVm.MainViewModel = this;
            var agendaViewModel = new AgendaWindowViewModel(fallbackHomeVm, null, 0);
            agendaViewModel.MainViewModel = this;
            CurrentViewModel = agendaViewModel;
        }
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
        switch (CurrentViewModel)
        {
            case AgendaWindowViewModel agendaViewModel:
                agendaViewModel.CurrentDay = selectedDate;
                agendaViewModel.SelectedIndex = 2;
                agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
                break;
            case HomeWindowViewModel homeViewModel:
            {
                var agendaViewModel = new AgendaWindowViewModel(homeViewModel, selectedDate, 2);
                agendaViewModel.MainViewModel = this;
                agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
                CurrentViewModel = agendaViewModel;
                break;
            }
            default:
            {
                var fallbackHomeVm = new HomeWindowViewModel();
                fallbackHomeVm.MainViewModel = this;
                var agendaViewModel = new AgendaWindowViewModel(fallbackHomeVm, selectedDate, 2);
                agendaViewModel.MainViewModel = this;
                agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
                CurrentViewModel = agendaViewModel;
                break;
            }
        }
    }


    
}
