using System;
using Agendai.Messages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;

namespace Agendai.Views.Components.AgendaSidebar;

public partial class AgendaSidebar : UserControl
{
    public AgendaSidebar()
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