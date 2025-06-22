using System;
using Agendai.Messages;
using Agendai.ViewModels.Agenda;
using CommunityToolkit.Mvvm.Messaging;


namespace Agendai.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	#region View-Model State

	private ViewModelBase? _currentViewModel;

	#endregion


	public MainWindowViewModel()
	{
		CurrentViewModel = new HomeWindowViewModel();

		if (CurrentViewModel is HomeWindowViewModel homeViewModel)
		{
			homeViewModel.MainViewModel = this;
		}

		WeakReferenceMessenger.Default.Register<NavigateToDateMessenger>(
			this,
			(_, message) => { NavigateToSpecificDay(message.SelectedDate); }
		);
	}


	#region State Tracking

	public ViewModelBase? CurrentViewModel
	{
		get => _currentViewModel;
		set => SetProperty(ref _currentViewModel, value);
	}

	#endregion


	#region Event Handlers

	public void NavigateToHome()
	{
		HomeWindowViewModel homeViewModel = new()
		{
			MainViewModel = this
		};

		CurrentViewModel = homeViewModel;
	}

	public void NavigateToAgenda()
	{
		if (CurrentViewModel is HomeWindowViewModel homeVm)
		{
			ResetFlags(homeVm);
			homeVm.IsAgendaWindow = true;

			AgendaWindowViewModel agendaViewModel = new(homeVm)
			{
				MainViewModel = this
			};

			CurrentViewModel = agendaViewModel;
		}
		else
		{
			HomeWindowViewModel fallbackHomeVm = new()
			{
				MainViewModel = this
			};

			ResetFlags(fallbackHomeVm);
			fallbackHomeVm.IsAgendaWindow = true;

			AgendaWindowViewModel agendaViewModel = new(fallbackHomeVm)
			{
				MainViewModel = this
			};

			CurrentViewModel = agendaViewModel;
		}
	}

	public void NavigateToTodo()
	{
		if (CurrentViewModel is HomeWindowViewModel homeVm)
		{
			ResetFlags(homeVm);
			homeVm.IsTodoWindow = true;
			var todoViewModel = homeVm.TodoWindowVm;

			if (todoViewModel is null) return;

			todoViewModel.EventListVm   = homeVm.EventListVm;
			todoViewModel.MainViewModel = this;
			CurrentViewModel            = todoViewModel;
		}
		else
		{
			HomeWindowViewModel fallbackHomeVm = new()
			{
				MainViewModel = this
			};

			ResetFlags(fallbackHomeVm);
			fallbackHomeVm.IsTodoWindow = true;

			TodoWindowViewModel todoViewModel = new()
			{
				MainViewModel = this
			};

			CurrentViewModel = todoViewModel;
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
			HomeWindowViewModel fallbackHomeVm = new()
			{
				MainViewModel = this
			};

			ResetFlags(fallbackHomeVm);
			fallbackHomeVm.IsPomodoroWindow = true;
		}

		PomodoroWindowViewModel pomodoroViewModel = new()
		{
			MainViewModel = this
		};

		CurrentViewModel = pomodoroViewModel;
	}

	private void NavigateToSpecificDay(DateTime selectedDate)
	{
		switch (CurrentViewModel)
		{
			case AgendaWindowViewModel agendaVm:
				if (agendaVm.HomeWindowVm is not null)
					agendaVm.HomeWindowVm.IsAgendaWindow = true;

				agendaVm.CurrentDay    = selectedDate;
				agendaVm.SelectedIndex = 2;
				agendaVm.DayController.UpdateDayFromDate(selectedDate);

				break;

			case HomeWindowViewModel homeVm:
			{
				ResetFlags(homeVm);
				homeVm.IsAgendaWindow = true;
				AgendaWindowViewModel agendaViewModel = new(homeVm, selectedDate, 2)
				{
					MainViewModel = this
				};

				agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
				CurrentViewModel = agendaViewModel;

				break;
			}

			default:
			{
				HomeWindowViewModel fallbackHomeVm = new()
				{
					MainViewModel = this
				};

				ResetFlags(fallbackHomeVm);
				fallbackHomeVm.IsAgendaWindow = true;
				AgendaWindowViewModel agendaViewModel = new(fallbackHomeVm, selectedDate, 2)
				{
					MainViewModel = this
				};
				agendaViewModel.DayController.UpdateDayFromDate(selectedDate);
				CurrentViewModel = agendaViewModel;

				break;
			}
		}
	}

	#endregion


	private static void ResetFlags(HomeWindowViewModel homeVm)
	{
		homeVm.IsAgendaWindow   = false;
		homeVm.IsTodoWindow     = false;
		homeVm.IsPomodoroWindow = false;
	}
}
