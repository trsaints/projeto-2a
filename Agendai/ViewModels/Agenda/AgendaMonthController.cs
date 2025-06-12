using System.Globalization;
using Agendai.Services.Views;

namespace Agendai.ViewModels.Agenda
{
    public class AgendaMonthController
    {
        private readonly AgendaWindowViewModel _viewModel;

        public AgendaMonthController(AgendaWindowViewModel viewModel)
        {
            _viewModel = viewModel;
        }

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

        public void UpdateMonthFromDate()
        {
            var culture = new CultureInfo("pt-BR");
            _viewModel.SelectedMonth = culture.TextInfo.ToTitleCase(_viewModel.CurrentMonth.ToString("MMMM", culture));

            MonthViewService.GenerateMonthView(
                _viewModel.MonthViewRows,
                _viewModel.EventList.Events,
                _viewModel.TodoList.Todos,
                _viewModel.CurrentMonth,
                _viewModel.ShowData,
                _viewModel.SelectedListNames
            );

            _viewModel.UpdateDataGridItems();
        }
    }
}