using System.ComponentModel;
using System;
using System.Collections.Generic;
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


	#region Utils

	public static List<Todo> Sample()
	{
		return
		[
			new Todo(1, "Comprar Pamonha")
			{
				Description = "Comprar pamonha na feira",
				Due         = DateTime.Today,
				Repeats     = Repeats.None,
				ListName    = "Compras",
				Status      = TodoStatus.Complete
			},
			new Todo(2, "Treino Fullbody")
			{
				Description = "Treino fullbody na feira",
				Due         = DateTime.Today,
				Repeats     = Repeats.Daily,
				ListName    = "Treinos"
			},
			new Todo(3, "Lavar o chão")
			{
				Description = "Lavar o chão da sala",
				Due         = DateTime.Today,
				Repeats     = Repeats.None,
				ListName    = "Casa"
			},
			new Todo(4, "Lavar o banheiro")
			{
				Description = "Lavar o banheiro",
				Due         = DateTime.Today,
				Repeats     = Repeats.None,
				ListName    = "Casa",
				Status      = TodoStatus.Complete
			}
		];
	}

	#endregion
}
