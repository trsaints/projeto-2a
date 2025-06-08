using System;
using System.Linq;
using Agendai.Messages;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Agendai.ViewModels;
using Avalonia.Interactivity;
using Agendai.Data;

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
        if (sender is CheckBox checkBox)
        {
            if (checkBox.DataContext is TodosByListName todoList)
            {
                if (checkBox.IsChecked == true)
                {
                    WeakReferenceMessenger.Default.Send(new GetListsNamesMessenger(new[] { todoList.ListName }));
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new GetListsNamesMessenger(Array.Empty<string>()));
                }
            }
        }
    }



}