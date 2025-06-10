using System.ComponentModel;
using System;

namespace Agendai.Data.Models;


public class Todo(ulong id, string name) : Recurrence(id, name), INotifyPropertyChanged
{
	public string?    ListName       { get; set; }
	public uint       FinishedShifts { get; set; }
	public uint       TotalShifts    { get; set; }
	public virtual Event? RelatedEvent { get; set; }
	
	private TodoStatus _status;
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
	
	public event PropertyChangedEventHandler PropertyChanged;
	public event Action<Todo, TodoStatus>? OnStatusChanged;

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
