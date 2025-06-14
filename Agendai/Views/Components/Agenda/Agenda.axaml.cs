using System;
using Agendai.Data.Models;
using Agendai.ViewModels.Agenda;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Agendai.Views.Components.Agenda;

public partial class Agenda : UserControl
{
    public Agenda()
    {
        InitializeComponent();
    }
    
    public void OnEventOrTodoCLicked(object? sender, RoutedEventArgs e)
    {
        var viewModel = DataContext as AgendaWindowViewModel;
        if (sender is Button button)
        {
            if (button.Tag is Event ev)
            {
                viewModel.EditEvent(ev);
            } 
            else if (button.Tag is Todo todo)
            {
                viewModel.EditTodo(todo);
            }
        }
    }
}