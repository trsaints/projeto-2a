using Agendai.ViewModels;
using Avalonia.Controls;


namespace Agendai.Views.Components.TodoList;


public partial class TodoList : UserControl
{
	public TodoList()
	{
		InitializeComponent();
		DataContext = new TodoListViewModel();
	}
}

