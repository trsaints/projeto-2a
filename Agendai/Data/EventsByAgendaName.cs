using System.Collections.Generic;
using Agendai.Data.Models;
using Agendai.ViewModels;

namespace Agendai.Data;

public class EventsByAgendaName : ViewModelBase
{
    public string AgendaName { get; set; }
    
    public IEnumerable<Event> Events { get; set; }
}