using System.Collections.ObjectModel;

namespace Agendai.Data;

public class DayRow
{
    public string Hour { get; set; }
    public ObservableCollection<string> Items { get; set; } = new();
}
