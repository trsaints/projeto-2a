using System.Globalization;
using Agendai.Services.Views;


namespace Agendai.ViewModels.Agenda;

public class AgendaMonthController
{
	#region Dependencies

	private readonly AgendaWindowViewModel _viewModel;

	#endregion


	public AgendaMonthController(AgendaWindowViewModel viewModel) { _viewModel = viewModel; }


	#region Event Handlers

	public void GoToPreviousMonth()
	{
		_viewModel.CurrentMonth = _viewModel.CurrentMonth.AddMonths(-1);
		UpdateMonthFromDate();
	}

	public void GoToNextMonth()
	{
		_viewModel.CurrentMonth = _viewModel.CurrentMonth.AddMonths(1);
		UpdateMonthFromDate();
	}

	private void UpdateMonthFromDate()
	{
		var culture = new CultureInfo("pt-BR");

		_viewModel.SelectedMonth =
				culture.TextInfo.ToTitleCase(
					_viewModel.CurrentMonth.ToString("MMMM", culture)
				);

		if (_viewModel is { EventList: not null, TodoList: not null })
			MonthViewService.GenerateMonthView(
				_viewModel.MonthViewRows,
				_viewModel.EventList.Events,
				_viewModel.TodoList.Todos,
				_viewModel.CurrentMonth,
				_viewModel.ShowData,
				_viewModel.SelectedListNames,
				_viewModel.SearchText
			);

		_viewModel.UpdateDataGridItems();
	}

	#endregion
}
