using Agendai.Data.Models;
using Avalonia.Controls;
using Agendai.ViewModels;
using Avalonia.Input;
using Avalonia.Interactivity;


namespace Agendai.Views.Components.EventList;

public partial class AddEvent : UserControl
{
	public AddEvent() { InitializeComponent(); }

	private void SearchText_OnGotFocus(object? sender, GotFocusEventArgs e)
	{
		if (sender is null) return;

		var autoComplete = (AutoCompleteBox)sender;
		autoComplete.IsDropDownOpen = true;
	}

	private void AddSearchBox_TextChanged(object? sender, RoutedEventArgs e)
	{
		if (sender is not AutoCompleteBox) return;

		if (DataContext is EventListViewModel) { }
	}

	private void RemoveRelatedTask(object sender, RoutedEventArgs e)
	{
		var button = (Button)sender;

		if (DataContext is EventListViewModel vm && button.DataContext is Todo todo)
		{
			vm.RemoveTodoFromEvent(todo);
		}
	}
}
