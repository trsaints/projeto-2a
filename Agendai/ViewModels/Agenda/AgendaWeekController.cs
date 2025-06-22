using Agendai.Services.Views;


namespace Agendai.ViewModels.Agenda;

public class AgendaWeekController
{
	#region Dependencies

	private readonly AgendaWindowViewModel _viewModel;

	#endregion


	public AgendaWeekController(AgendaWindowViewModel viewModel) { _viewModel = viewModel; }

	public void GoToPreviousWeek()
	{
		_viewModel.CurrentWeek = _viewModel.CurrentWeek.AddDays(-7);
		UpdateWeekFromDate();
	}

	public void GoToNextWeek()
	{
		_viewModel.CurrentWeek = _viewModel.CurrentWeek.AddDays(7);
		UpdateWeekFromDate();
	}

	private void UpdateWeekFromDate()
	{
		var (weekNumber, start, end) =
				WeekViewService.GetWeekOfMonthRange(_viewModel.CurrentWeek);
		_viewModel.SelectedWeek = $"Semana {weekNumber} - {start:dd/MM} a {end:dd/MM}";

		if (_viewModel is { EventList: not null, TodoList: not null })
			WeekViewService.GenerateWeekView(
				_viewModel.WeekViewRows,
				AgendaWindowViewModel.Hours,
				_viewModel.EventList.Events,
				_viewModel.TodoList.Todos,
				_viewModel.CurrentWeek,
				_viewModel.ShowData,
				_viewModel.SelectedListNames,
				_viewModel.SearchText
			);

		_viewModel.UpdateDataGridItems();
	}
}
