using System.ComponentModel;

namespace Agendai.Data.Models;


public class Event(ulong id, string name) : Recurrence(id, name), INotifyPropertyChanged
{
	public string? AgendaName { get; set; }
	
	public event PropertyChangedEventHandler? PropertyChanged;
	
	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
