using Agendai.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Agendai.Views.Components.Agenda;

public partial class DayView : UserControl
{
    public DayView()
    {
        InitializeComponent();
    }
    
    private void OnPreviousDayClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AgendaWindowViewModel vm)
            vm.GoToPreviousDay();
    }

    private void OnNextDayClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AgendaWindowViewModel vm)
            vm.GoToNextDay();
    }
}