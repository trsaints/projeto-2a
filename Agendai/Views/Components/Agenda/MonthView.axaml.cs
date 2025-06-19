using Agendai.Data.Models;
using Agendai.ViewModels.Agenda;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Agendai.Views.Components.Agenda;

public partial class MonthView : UserControl
{
    public MonthView()
    {
        InitializeComponent();
    }

    private void OnPreviousMonthClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AgendaWindowViewModel vm)
            vm.GoToPreviousMonth();
    }

    private void OnNextMonthClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AgendaWindowViewModel vm)
            vm.GoToNextMonth();
    }

    private void OnDayClicked(object? sender, RoutedEventArgs e)
    {
        
        if (e.Source is Control source && (source.DataContext is Todo || source.DataContext is Event))
        {
            return;
        }
        
        if (sender is Border border && border.Tag is int clickedDate)
        {
            if (DataContext is AgendaWindowViewModel vm)
            {
                vm.GoToDay(clickedDate);
            }
        }
    }

    private void ForwardClickToParent(object? sender, RoutedEventArgs e)
    {
        var parentAgenda = this.FindAncestorOfType<Agenda>();
        parentAgenda?.OnEventOrTodoCLicked(sender, e);
    }

}
