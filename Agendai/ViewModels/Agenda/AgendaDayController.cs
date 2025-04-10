using System.Globalization;
using Agendai.Services.Views;

namespace Agendai.ViewModels.Agenda
{
    public class AgendaDayController
    {
        private readonly AgendaWindowViewModel _viewModel;

        public AgendaDayController(AgendaWindowViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void GoToPreviousDay()
        {
            _viewModel.CurrentDay = _viewModel.CurrentDay.AddDays(-1);
            _viewModel.UpdateDataGridItems();
            UpdateDayFromDate();
        }

        public void GoToNextDay()
        {
            _viewModel.CurrentDay = _viewModel.CurrentDay.AddDays(1);
            _viewModel.UpdateDataGridItems();
            UpdateDayFromDate();
        }

        public void UpdateDayFromDate()
        {
            var culture = new CultureInfo("pt-BR");
            _viewModel.SelectedDay = culture.TextInfo.ToTitleCase(_viewModel.CurrentDay.ToString("dddd, dd 'de' MMMM", culture));
            
            var mappedItems = DayViewService.MapDayItemsFrom(
                _viewModel.EventList.Events,
                _viewModel.TodoList.Todos,
                _viewModel.CurrentDay
            );

            DayViewService.GenerateDayView(
                _viewModel.DayViewRows,
                _viewModel.Hours,
                mappedItems
            );
            
            _viewModel.UpdateDataGridItems();
        }
    }
}