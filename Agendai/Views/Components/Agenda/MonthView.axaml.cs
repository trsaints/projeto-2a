using System;
using System.Linq;
using Agendai.Data.Models;
using Agendai.ViewModels.Agenda;
using Avalonia.Controls;
using Avalonia.Input;
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

    private void SearchText_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        var autoComplete = (AutoCompleteBox)sender;
        autoComplete.IsDropDownOpen = true;
    }
    
    private void SearchBox_TextChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AutoCompleteBox box)
        {
            if (this.DataContext is AgendaWindowViewModel vm)
            {
                vm.SearchText = box.Text;
            }
        }
    }
    
    private void SearchBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is AutoCompleteBox box && box.SelectedItem is string selectedName)
        {
            var viewModel = DataContext as AgendaWindowViewModel;

            var ev = viewModel?.EventList.Events.FirstOrDefault(x => x.Name == selectedName);
            var todo = viewModel?.TodoList.Todos.FirstOrDefault(x => x.Name == selectedName);

            if (ev != null || todo != null)
            {
                var fakeButton = new Button { Tag = ev ?? (object)todo };
                var parentAgenda = this.FindAncestorOfType<Agenda>();

                parentAgenda?.OnEventOrTodoCLicked(fakeButton, e);
            }
        }
    }


    
}
