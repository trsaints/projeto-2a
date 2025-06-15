using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Agendai.Models;
using Avalonia;


namespace Agendai.Views.Components.TodoList;

public partial class TodoList : UserControl
{
	public static readonly StyledProperty<ObservableCollection<Todo>>
		ItemsSourceProperty =
			AvaloniaProperty.Register<TodoList, ObservableCollection<Todo>>(
				nameof(ItemsSource)
			);
	
	public static readonly StyledProperty<ICommand> ItemClickCommandProperty =
		AvaloniaProperty.Register<TodoList, ICommand>(nameof(ItemClickCommand));

	public ObservableCollection<Todo> ItemsSource
	{
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}
	
	public ICommand ItemClickCommand
	{
		get => GetValue(ItemClickCommandProperty);
		set => SetValue(ItemClickCommandProperty, value);
	}

	public TodoList()
	{
		InitializeComponent();
		
	}
}
