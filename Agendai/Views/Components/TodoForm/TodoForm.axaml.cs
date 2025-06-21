using Agendai.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;


namespace Agendai.Views.Components.TodoForm;

public partial class TodoForm : UserControl
{
	public TodoForm()
	{
		InitializeComponent();
		DataContext = new TodoWindowViewModel();
	}

	private void TodoListName_OnGotFocus(object? sender, GotFocusEventArgs e)
	{
		var autoComplete = (AutoCompleteBox)sender;
		autoComplete.IsDropDownOpen = true;
	}
}
