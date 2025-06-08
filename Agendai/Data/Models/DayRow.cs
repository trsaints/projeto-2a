using System.Collections.ObjectModel;

namespace Agendai.Data.Models;

public class DayRow
{
    public string Hour { get; set; }
    public ObservableCollection<object> Items { get; set; } = new();
}
