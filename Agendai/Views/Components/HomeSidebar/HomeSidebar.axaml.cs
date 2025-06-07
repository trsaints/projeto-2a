using System;
using Agendai.Navigators;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;

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
}