using System;
using System.Collections.Generic;


namespace Agendai.Data.Models;

public abstract class Recurrence : Entity
{
	private Repeats _repeats;
	public Repeats Repeats
	{
		get => _repeats;
		set => SetProperty(ref _repeats, value);
	}

	private IEnumerable<DateTime>? _reminders;
	public IEnumerable<DateTime>? Reminders
	{
		get => _reminders;
		set => SetProperty(ref _reminders, value);
	}

	private DateTime _initialDue = DateTime.Now;
	public DateTime InitialDue
	{
		get => _initialDue;
		set => SetProperty(ref _initialDue, value);
	}

	private DateTime _due = DateTime.Now;
	public DateTime Due
	{
		get => _due;
		set => SetProperty(ref _due, value);
	}

	private string? _description;
	public string? Description
	{
		get => _description;
		set => SetProperty(ref _description, value);
	}

	protected Recurrence(ulong id, string name) : base(id, name) { }
}
