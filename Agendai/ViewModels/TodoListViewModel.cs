using System;
using System.Collections.ObjectModel;
using Agendai.Models;


namespace Agendai.ViewModels;


public class TodoListViewModel : ViewModelBase
{
	public TodoListViewModel()
	{
		Todos = [
			new Todo(1, "Comprar Café")
			{
				Description = "Preciso comprar café",
				Due = new DateTime(2025, 03, 31),
				Repeats = Repeats.Monthly
			},
			new Todo(2, "Comprar Pão")
			{
				Description = "Preciso comprar pão",
				Due = new DateTime(2025, 03, 31),
				Repeats = Repeats.Weekly
			},
			new Todo(3, "Comprar Leite")
			{
				Description = "Preciso comprar leite",
				Due = new DateTime(2025, 03, 31),
				Repeats = Repeats.Daily
			}
		];
	}
	
	public ObservableCollection<Todo> Todos { get; set; }

	public bool IsComplete(Todo todo)
	{
		return todo.Status is TodoStatus.Complete;
	}
}
