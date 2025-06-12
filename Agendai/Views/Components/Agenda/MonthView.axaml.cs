using System;
using Agendai.ViewModels;
using Agendai.ViewModels.Agenda;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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
        if (sender is Button button && button.Tag is int clickedDate)
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
