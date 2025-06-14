﻿using Agendai.ViewModels;
using Agendai.ViewModels.Agenda;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace Agendai.Views.Components.Agenda;

public partial class WeekView : UserControl
{
    public WeekView()
    {
        InitializeComponent();
    }
    
    private void OnPreviousWeekClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AgendaWindowViewModel vm)
            vm.GoToPreviousWeek();
    }

    private void OnNextWeekClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AgendaWindowViewModel vm)
            vm.GoToNextWeek();
    }
    
    private void ForwardClickToParent(object? sender, RoutedEventArgs e)
    {
        var parentAgenda = this.FindAncestorOfType<Agenda>();
        parentAgenda?.OnEventOrTodoCLicked(sender, e);
    }

}