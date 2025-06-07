using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;


public class Recurrence(ulong id, string name) : Entity(id, name)
{
	[Required]
	public Repeats                Repeats    { get; set; }

	[NotMapped]
	public IEnumerable<DateTime>? Reminders  { get; set; }
	public DateTime               InitialDue { get; set; } = DateTime.Now;
	public DateTime               Due        { get; set; } = DateTime.Now;

	[StringLength(256)]
	public string?                Description { get; set; }
}
