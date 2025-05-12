using System;
using Agendai.Messages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;


namespace Agendai.Views.Components.TodoSidebar;

public partial class TodoSidebar : UserControl
{
	public TodoSidebar() { InitializeComponent(); }
	
	private void OnCalendarDateChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count > 0 && e.AddedItems[0] is DateTime selectedDate)
		{
			WeakReferenceMessenger.Default.Send(new NavigateToDateMessenger(selectedDate));
		}
	}
}

