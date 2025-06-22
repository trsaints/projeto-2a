using System.Collections.ObjectModel;
using Avalonia.Controls;
using Agendai.Data.Models;
using Avalonia;


namespace Agendai.Views.Components.TodoList;

public partial class TodoList : UserControl
{
	public static readonly StyledProperty<ObservableCollection<Todo>>
			ItemsSourceProperty =
					AvaloniaProperty.Register<TodoList, ObservableCollection<Todo>>(
						nameof(ItemsSource)
					);

	public ObservableCollection<Todo> ItemsSource
	{
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public TodoList() { InitializeComponent(); }
}
