﻿using System;
using System.Globalization;
using Agendai.Controllers.Interfaces;
using Agendai.Services;
using Agendai.ViewModels;


namespace Agendai.Controllers;

public class AgendaDayController : IAgendaDayController
{
	#region Dependencies

	private readonly AgendaWindowViewModel _viewModel;

	#endregion


	public AgendaDayController(AgendaWindowViewModel viewModel) { _viewModel = viewModel; }


	#region Event Handlers

	public void GoToPreviousDay()
	{
		_viewModel.CurrentDay = _viewModel.CurrentDay.AddDays(-1);
		UpdateDayFromDate(_viewModel.CurrentDay);
	}

	public void GoToNextDay()
	{
		_viewModel.CurrentDay = _viewModel.CurrentDay.AddDays(1);
		UpdateDayFromDate(_viewModel.CurrentDay);
	}

	public void UpdateDayFromDate(DateTime selectedDate)
	{
		var culture = new CultureInfo("pt-BR");
		_viewModel.SelectedDay = culture.TextInfo.ToTitleCase(
			_viewModel.CurrentDay.ToString("dddd, dd 'de' MMMM", culture)
		);

		if (_viewModel is { EventList: not null, TodoList: not null })
			DayViewService.MapDayItemsFrom(
				_viewModel.EventList.Events,
				_viewModel.TodoList.Todos,
				selectedDate,
				_viewModel.SelectedListNames
			);

		_viewModel.UpdateDataGridItems();
	}

	public void GoToDay(int dayNumber)
	{
		var selectedDate = new DateTime(
			_viewModel.CurrentMonth.Year,
			_viewModel.CurrentMonth.Month,
			dayNumber
		);
		_viewModel.CurrentDay    = selectedDate;
		_viewModel.SelectedIndex = 2;
		UpdateDayFromDate(selectedDate);
	}

	#endregion
}
