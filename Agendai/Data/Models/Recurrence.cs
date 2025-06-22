using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;

public class Recurrence : Entity
{
	#region Entity State

	[NotMapped]
	private Repeats _repeats = Repeats.None;

	[NotMapped]
	private DateTime _initialDue = DateTime.Now;

	[NotMapped]
	private DateTime _due = DateTime.Now;

	[NotMapped]
	private string? _description = string.Empty;

	#endregion


	protected Recurrence() { }
	protected Recurrence(int id, string name) : base(id, name) { }


	#region State Tracking

	[DefaultValue(Repeats.None)]
	public Repeats Repeats
	{
		get => _repeats;
		set => SetProperty(ref _repeats, value);
	}

	public DateTime InitialDue
	{
		get => _initialDue;
		set => SetProperty(ref _initialDue, value);
	}

	[Required]
	public DateTime Due
	{
		get => _due;
		set => SetProperty(ref _due, value);
	}

	[StringLength(256)]
	public string? Description
	{
		get => _description;
		set => SetProperty(ref _description, value);
	}

	#endregion
}
