namespace Agendai.Models;


public class Event(ulong id, string name) : Recurrence(id, name)
{
	public string? AgendaName { get; set; }
}
