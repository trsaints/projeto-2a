using System;
using System.Collections.Generic;


namespace Agendai.Models;


public abstract class Recurrence(ulong id, string name) : Entity(id, name)
{
	public Repeats                Repeats    { get; set; }
	public IEnumerable<DateTime>? Reminders  { get; set; }
	public DateTime               InitialDue { get; set; } = DateTime.Now;
	public DateTime               Due        { get; set; } = DateTime.Now;
	public string?                Description { get; set; }
}
