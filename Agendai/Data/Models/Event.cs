using Avalonia.Media;
using System.Collections.Generic;
using System.ComponentModel;

namespace Agendai.Data.Models;


public class Event(ulong id, string name) : Recurrence(id, name), INotifyPropertyChanged
{
	public string? AgendaName { get; set; }
	
	public virtual ICollection<Todo>? Todos { get; set; }
	
	public event PropertyChangedEventHandler? PropertyChanged;

    private string? _color = "#FFB900";
    public string? Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ColorBrush));
            }
        }
    }

    public IBrush ColorBrush => SolidColorBrush.Parse(Color ?? "#FFFFFF");

    protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
