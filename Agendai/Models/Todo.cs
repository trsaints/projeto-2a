using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace Agendai.Models;

public class Todo(ulong id, string name) : Recurrence(id, name)
{
    public string? ListName { get; set; }
    public uint FinishedShifts { get; set; }
    public uint TotalShifts { get; set; }
    
    // Adicionando coleção de Shifts
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public Shift Shift {get; set;}

    public ulong ShiftId {get; set;}
    
    public TodoStatus Status { get; set; }

    public Event? Event { get; set; }
    public ulong? EventId { get; set; }
}
