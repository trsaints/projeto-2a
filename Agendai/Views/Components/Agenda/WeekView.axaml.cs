using System.Linq;
using Agendai.ViewModels;
using Agendai.ViewModels.Agenda;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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