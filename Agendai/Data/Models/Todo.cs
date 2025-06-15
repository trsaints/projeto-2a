using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;

namespace Agendai.Data.Models;


[Table("Todos")]
public class Todo : Recurrence, INotifyPropertyChanged
{
	public Todo(ulong id, string name) : base(id, name)
    {
    }

    public Todo() { }

	[StringLength(64)]
    public string?    ListName       { get; set; }

	[DefaultValue(0)]
	public uint       FinishedShifts { get; set; }

	[DefaultValue(0)]
	public uint       TotalShifts    { get; set; }

	[ForeignKey(nameof(Event))]
	public ulong? EventId { get; set; }
    public virtual Event? Event { get; set; }

	[NotMapped]
	private TodoStatus _status;

	[DefaultValue(TodoStatus.Incomplete)]
    public TodoStatus Status
	{
		get => _status;
		set
		{
			if (_status != value)
			{
				_status = value;
				OnPropertyChanged(nameof(Status));
				OnStatusChanged?.Invoke(this, _status);
			}
		}
	}

	public virtual ICollection<Shift> Shifts { get; set; } = [];
	
	public new event PropertyChangedEventHandler? PropertyChanged;
	public event Action<Todo, TodoStatus>? OnStatusChanged;

	protected new virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
