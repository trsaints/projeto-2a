using System;


namespace Agendai.Models;


public class Shift(ulong id, string name) : Entity(id, name)
{
	public TimeOnly Duration { get; set; }
	public ShiftStatus Status { get; set; }
}
