using Avalonia.Media;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;

[Table("Events")]
public class Event : Recurrence
{
	#region Entity State

	[NotMapped]
	private string? _color = "#FFB900";

	#endregion


	[StringLength(64)]
	public string? AgendaName { get; set; }

	public virtual ICollection<Todo> Todos { get; set; } = [];

	public Event() { }
	public Event(int id, string name) : base(id, name) { }


	#region State Tracking

	[Required]
	public string? Color
	{
		get => _color;

		set
		{
			if (_color == value) return;

			_color = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(ColorBrush));
		}
	}

	public IBrush ColorBrush => SolidColorBrush.Parse(Color ?? "#FFFFFF");

	#endregion
}
