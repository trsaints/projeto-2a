using System.Collections.Generic;
using System.ComponentModel;


namespace Agendai.Data.Models;

public class Event(ulong id, string name)
		: Recurrence(id, name), INotifyPropertyChanged
{
	public string? AgendaName { get; set; }

	public virtual ICollection<Todo>? Todos { get; set; }

	public new event PropertyChangedEventHandler? PropertyChanged;

	protected new virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(
			this,
			new PropertyChangedEventArgs(propertyName)
		);
	}

	public static List<Event> Samples()
	{
		return
		[
			new Event(1, "Evento 1") { AgendaName = "Agenda Pessoal" },
			new Event(2, "Evento 2") { AgendaName = "Agenda Pessoal" },
			new Event(3, "Evento 3") { AgendaName = "Agenda Pessoal" },
			new Event(4, "Evento 4")
					{ AgendaName = "Agenda de Trabalho" },
			new Event(5, "Evento 5")
					{ AgendaName = "Agenda de Trabalho" },
			new Event(6, "Evento 6")
					{ AgendaName = "Agenda de Trabalho" },
			new Event(7, "Evento 7")
					{ AgendaName = "Agenda de Estudos" },
			new Event(8, "Evento 8")
					{ AgendaName = "Agenda de Estudos" },
			new Event(9, "Evento 9")
					{ AgendaName = "Agenda de Estudos" },
			new Event(10, "Evento 10")
					{ AgendaName = "Agenda de Estudos" }
		];
	}
}
