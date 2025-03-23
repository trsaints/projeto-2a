namespace Agendai.Models;


public class Todo(ulong id, string name) : Recurrence(id, name)
{
	public string?    ListName       { get; set; }
	public TodoStatus Status         { get; set; }
	public uint       FinishedShifts { get; set; }
	public uint       TotalShifts    { get; set; }
}
