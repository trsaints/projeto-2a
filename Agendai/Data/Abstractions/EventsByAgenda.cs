using System.Collections.Generic;
using Agendai.Data.Models;


namespace Agendai.Data.Abstractions;

public class EventsByAgenda
{
	public string AgendaName { get; set; }

	public IEnumerable<Event> Events { get; set; }
}
