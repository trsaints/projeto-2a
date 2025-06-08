using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agendai.Data.Models;

[Table("Todos")]
public class Todo() : Recurrence, INotifyPropertyChanged
{
    [StringLength(64)]
    public string? ListName { get; set; }

    [DefaultValue(0)]
    public uint FinishedShifts { get; set; }
    public uint TotalShifts { get; set; }

    private TodoStatus _status;
    public TodoStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnStatusChanged?.Invoke(this, _status);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<Todo, TodoStatus>? OnStatusChanged;

    [ForeignKey(nameof(Event))]
    public ulong? EventId { get; set; }
    public virtual Event? Event { get; set; }

    public virtual ICollection<Shift>? Shifts { get; set; } = [];

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
