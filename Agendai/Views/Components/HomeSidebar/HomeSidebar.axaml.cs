using System;
using Agendai.Messages;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Agendai.ViewModels;
using Avalonia.Interactivity;

namespace Agendai.Views.Components.HomeSidebar;

public partial class HomeSidebar : UserControl
{
    public HomeSidebar()
    {
        InitializeComponent();
    }
    
    private void OnCalendarDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is DateTime selectedDate)
        {
            WeakReferenceMessenger.Default.Send(new NavigateToDateMessenger(selectedDate));
        }
    }

    private void OnCheckBoxChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.DataContext is TodoWindowViewModel todo)
        {
            Console.WriteLine($"Item selecionado: {todo.ListName} (Checked)");
        }
    }
}