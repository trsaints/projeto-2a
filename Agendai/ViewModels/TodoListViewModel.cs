using System.Collections.ObjectModel;
using Agendai.Models;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Agendai.ViewModels;

public class TodoListViewModel : ObservableObject
{
	private ObservableCollection<Todo>? _items;

	public ObservableCollection<Todo> Items
	{
		get => _items ?? [];
		set => SetProperty(ref _items, value);
	}
}
