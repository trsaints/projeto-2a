using System.Collections.ObjectModel;

namespace Agendai.Models;

public class DayRow
{
    public string Hour { get; set; }
    public ObservableCollection<string> Items { get; set; } = new();
}
