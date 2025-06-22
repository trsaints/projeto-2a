using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;

[Table("Shifts")]
public class Shift : Entity
{
	public Shift() { }
	public Shift(int id, string name) : base(id, name) { }

	[DefaultValue(ShiftStatus.Incomplete)]
	public ShiftStatus Status { get; set; }

	[Required]
	[Range(0, 90)]
	public TimeOnly Duration { get; set; }
}
