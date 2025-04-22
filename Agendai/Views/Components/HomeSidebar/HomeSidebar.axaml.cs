using Agendai.ViewModels;
using Avalonia.Controls;

namespace Agendai.Views.Components.HomeSidebar;

public partial class HomeSidebar : UserControl
{
    public HomeSidebar()
    {
        InitializeComponent();
    }
    
    private void OnCalendarDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel mainViewModel)
        {
            var calendar = sender as Calendar;
            if (calendar?.SelectedDate.HasValue == true)
            {
                var selectedDate = calendar.SelectedDate.Value;
                mainViewModel.NavigateToSpecificDay(selectedDate);
            }
        }
    }
}