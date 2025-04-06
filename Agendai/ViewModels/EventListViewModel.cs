using System;
using System.Collections.ObjectModel;
using Agendai.Models;


namespace Agendai.ViewModels;


public class EventListViewModel : ViewModelBase
{
	public EventListViewModel()
	{
		Events =
		[
			new Event(1, "Conferência da Pamonha")
			{
				Description = "Conferência de pamonhas",
				Due = new DateTime(2025, 4, 6, 8, 0, 0),
				Repeats = Repeats.Anually
			},
			new Event(2, "Feira da Foda")
			{
				Description = "Conferência do Agro",
				Due = new DateTime(2025, 4, 5),
				Repeats = Repeats.None
			},
			new Event(3, "Festa do Peão")
			{
				Description = "Festa do Peão de Barretos",
				Due = new DateTime(2021, 8, 20),
				Repeats = Repeats.Monthly
			},
			new Event(4, "Feriado")
			{
				Description = "Feriado de alguma coisa",
				Due = new DateTime(2025, 4, 18),
				Repeats = Repeats.Monthly
			}
		];
	}
	
	public ObservableCollection<Event> Events { get; set; }
}
