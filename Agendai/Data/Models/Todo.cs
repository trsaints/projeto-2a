using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;

[Table("Todos")]
public class Todo : Recurrence
{
	#region Entity State

	[NotMapped]
	private TodoStatus _status;

	#endregion


	#region State Tracking

	[DefaultValue(TodoStatus.Incomplete)]
	public TodoStatus Status
	{
		get => _status;

		set
		{
			if (_status == value) return;

			_status = value;
			OnPropertyChanged();
			OnStatusChanged?.Invoke(this, _status);
		}
	}

	#endregion


	[StringLength(64)]
	public string? ListName { get; set; }

	public uint FinishedShifts { get; set; }
	public uint TotalShifts    { get; set; }

	[ForeignKey(nameof(Event))]
	public int? EventId { get;         set; }
	public virtual Event? Event { get; set; }

	public Todo() { }
	public Todo(int id, string name) : base(id, name) { }


	public event Action<Todo, TodoStatus>? OnStatusChanged;
}
