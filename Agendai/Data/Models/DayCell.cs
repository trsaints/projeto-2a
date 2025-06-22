using System.Collections.ObjectModel;


namespace Agendai.Data.Models;

public class DayCell
{
	public int?                         DayNumber { get; set; }
	public ObservableCollection<object> Items     { get; set; } = new();
}
