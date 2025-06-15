using System.Collections.Generic;
using System.ComponentModel;

namespace Agendai.Data.Models;


public class Event : Recurrence, INotifyPropertyChanged
{
	public Event(ulong id, string name) : base(id, name)
    {
    }

    public Event() { }

    public string? AgendaName { get; set; }
	
	public virtual ICollection<Todo>? Todos { get; set; }
	
	public new event PropertyChangedEventHandler? PropertyChanged;
	
	protected new virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
