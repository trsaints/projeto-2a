using System;
using System.Linq;
using Agendai.Data.Models;
using Avalonia;
using Avalonia.Controls;
using Agendai.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;


namespace Agendai.Views.Components.EventList;

public partial class AddEvent : UserControl
{
	public AddEvent() { InitializeComponent(); }

	private void SearchText_OnGotFocus(object? sender, GotFocusEventArgs e)
	{
		var autoComplete = (AutoCompleteBox)sender;
		autoComplete.IsDropDownOpen = true;
	}

	private void AddSearchBox_TextChanged(object? sender, RoutedEventArgs e)
	{
		if (sender is AutoCompleteBox box)
		{
			if (this.DataContext is EventListViewModel vm) { }
		}
	}

	private void RemoveRelatedTask(object sender, RoutedEventArgs e)
	{
		var button = (Button)sender;
		var todo   = button.DataContext as Todo;

		if (DataContext is EventListViewModel vm && todo != null)
		{
			vm.RemoveTodoFromEvent(todo);
		}
	}
}
