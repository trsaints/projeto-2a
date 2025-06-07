using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agendai.Data.Models;

[Table("Events")]
public class Event(ulong id, string name) : Recurrence(id, name)
{
	[StringLength(64)]
	public string? AgendaName { get; set; }
	public virtual ICollection<Todo>? Todos { get; set; } = [];
}
