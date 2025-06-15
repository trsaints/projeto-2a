using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Agendai.Data.Models;


public class Recurrence : Entity
{
    #region State-tracking Properties
    [NotMapped]
    private Repeats _repeats;

    [NotMapped]
    private IEnumerable<DateTime>? _reminders;

    [NotMapped]
    private DateTime _initialDue = DateTime.Now;

    [NotMapped]
    private DateTime _due = DateTime.Now;

    [NotMapped]
    private string? _description;
    #endregion

    public Recurrence(ulong id, string name) : base(id, name)
    {
    }

    public Recurrence() { }

    [Required]
    [DefaultValue(Repeats.None)]
    public Repeats Repeats
    {
        get => _repeats;
        set => SetProperty(ref _repeats, value);
    }

    public IEnumerable<DateTime>? Reminders
    {
        get => _reminders;
        set => SetProperty(ref _reminders, value);
    }

    [Required]
    public DateTime InitialDue
    {
        get => _initialDue;
        set => SetProperty(ref _initialDue, value);
    }


    [Required]
    public DateTime Due
    {
        get => _due;
        set => SetProperty(ref _due, value);
    }


    [MaxLength(256)]
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
}
