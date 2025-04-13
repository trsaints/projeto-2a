using System.Collections.ObjectModel;
using Avalonia.Controls;
using Agendai.Models;
using Avalonia;


namespace Agendai.Views.Components.TodoList;

public partial class TodoList : UserControl
{
	public static readonly StyledProperty<ObservableCollection<Todo>>
			ItemsProperty =
					AvaloniaProperty
							.Register<TodoList, ObservableCollection<Todo>>(
								nameof(Items)
							);

	public ObservableCollection<Todo> Items
	{
		get => GetValue(ItemsProperty);
		set => SetValue(ItemsProperty, value);
	}

	public TodoList()
	{
		InitializeComponent();
	}
}
