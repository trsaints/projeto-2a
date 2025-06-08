using System;
using Agendai.Data.Models;
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
        if (sender is Button button)
        {
            if (button.Tag is Event ev)
            {
                Console.WriteLine($"Evento clicado: {ev.Name}");
            } 
            else if (button.Tag is Todo todo)
            {
                Console.WriteLine($"Todo clicado: {todo.Name}");
            }
        }
    }
}