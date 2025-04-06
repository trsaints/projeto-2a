using System.Collections.ObjectModel;

namespace Agendai.Models;

public class DayCell
{
    public int? DayNumber { get; set; }
    public ObservableCollection<string> Items { get; set; } = new();
}