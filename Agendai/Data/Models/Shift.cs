using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;

[Table("Shifts")]
public class Shift : Entity, INotifyPropertyChanged
{
	public Shift(ulong id, string name) : base(id, name)
    {
    }

    public Shift() { }

    [Required]
    [MinLength(30)]
    public TimeOnly Duration { get; set; }

    [DefaultValue(ShiftStatus.Incomplete)]
	public ShiftStatus Status { get; set; }

    [ForeignKey(nameof(Todo))]
    public ulong? TodoId { get; set; }
    public virtual Todo? Todo { get; set; }
}
