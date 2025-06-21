using System;
using Agendai.Data.Models;
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

		WeakReferenceMessenger.Default.Register<NavigateToDateMessenger>(
			this,
			(r, message) => { NavigateToSpecificDay(message.SelectedDate); }
		);
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
		CurrentViewModel            = homeViewModel;
	}

	public void NavigateToAgenda()
	{
		if (CurrentViewModel is HomeWindowViewModel homeVm)
		{
			ResetFlags(homeVm);
			homeVm.IsAgendaWindow = true;
			var agendaViewModel = new AgendaWindowViewModel(homeVm, null, 0);
			agendaViewModel.MainViewModel = this;
			CurrentViewModel              = agendaViewModel;
		}
		else
		{
			var fallbackHomeVm = new HomeWindowViewModel();
			fallbackHomeVm.MainViewModel = this;
			ResetFlags(fallbackHomeVm);
			fallbackHomeVm.IsAgendaWindow = true;
			var agendaViewModel =
					new AgendaWindowViewModel(fallbackHomeVm, null, 0);
			agendaViewModel.MainViewModel = this;
			CurrentViewModel              = agendaViewModel;
		}
	}

	public void NavigateToTodo()
	{
		if (CurrentViewModel is HomeWindowViewModel homeVm)
		{
			ResetFlags(homeVm);
			homeVm.IsTodoWindow = true;
			var todoViewModel = homeVm.TodoWindowVm;
			todoViewModel.EventListVm   = homeVm.EventListVm;
			todoViewModel.MainViewModel = this;
			CurrentViewModel            = todoViewModel;
		}
		else
		{
			var fallbackHomeVm = new HomeWindowViewModel();
			fallbackHomeVm.MainViewModel = this;
			ResetFlags(fallbackHomeVm);
			fallbackHomeVm.IsTodoWindow = true;
			var todoViewModel = new TodoWindowViewModel();
			todoViewModel.MainViewModel = this;
			CurrentViewModel            = todoViewModel;
		}
	}

	public void NavigateToPomodoro()
	{
		if (CurrentViewModel is HomeWindowViewModel homeVm)
		{
			ResetFlags(homeVm);
			homeVm.IsPomodoroWindow = true;
		}
		else
		{
			var fallbackHomeVm = new HomeWindowViewModel();
			fallbackHomeVm.MainViewModel = this;
			ResetFlags(fallbackHomeVm);
			fallbackHomeVm.IsPomodoroWindow = true;
		}

		var pomodoroViewModel = new PomodoroWindowViewModel();
		pomodoroViewModel.MainViewModel = this;
		CurrentViewModel                = pomodoroViewModel;
	}

	private void ResetFlags(HomeWindowViewModel homeVm)
	{
		homeVm.IsAgendaWindow   = false;
		homeVm.IsTodoWindow     = false;
		homeVm.IsPomodoroWindow = false;
	}


	public void NavigateToSpecificDay(DateTime selectedDate)
	{
		switch (CurrentViewModel)
		{
			case AgendaWindowViewModel agendaVm:
				agendaVm.HomeWindowVm.IsAgendaWindow = true;
				agendaVm.CurrentDay                  = selectedDate;
				agendaVm.SelectedIndex               = 2;
				agendaVm.DayController.UpdateDayFromDate(selectedDate);

				break;

			case HomeWindowViewModel homeVm:
			{
				ResetFlags(homeVm);
				homeVm.IsAgendaWindow = true;
				var agendaViewModel =
						new AgendaWindowViewModel(homeVm, selectedDate, 2);
				agendaViewModel.MainViewModel = this;
				agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
				CurrentViewModel = agendaViewModel;

				break;
			}

			default:
			{
				var fallbackHomeVm = new HomeWindowViewModel();
				fallbackHomeVm.MainViewModel = this;
				ResetFlags(fallbackHomeVm);
				fallbackHomeVm.IsAgendaWindow = true;
				var agendaViewModel = new AgendaWindowViewModel(
					fallbackHomeVm,
					selectedDate,
					2
				);
				agendaViewModel.MainViewModel = this;
				agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
				CurrentViewModel = agendaViewModel;

				break;
			}
		}
	}
}
