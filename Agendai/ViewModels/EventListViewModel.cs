using System;
using System.Collections.ObjectModel;
using Agendai.Data.Models;


namespace Agendai.ViewModels;


public class EventListViewModel : ViewModelBase
{
    public EventListViewModel()
    {
    }

    public Action? OnEventAdded { get; set; }

    public ObservableCollection<Event> Events { get; set; } = [];
    public ObservableCollection<Repeats> RepeatOptions { get; } =
    [
        Repeats.None,
        Repeats.Daily,
        Repeats.Weekly,
        Repeats.Monthly,
        Repeats.Anually
    ];

    private string _newEventName;
    public string NewEventName
    {
        get => _newEventName;
        set
        {
            _newEventName = value;
            OnPropertyChanged();
        }
    }

    private DateTime _newDue;

    public DateTime NewDue
    {
        get => _newDue;
        set
        {
            _newDue = value;
            OnPropertyChanged();
        }
    }

    private string _newDescription;
    public string NewDescription
    {
        get => _newDescription;
        set
        {
            _newDescription = value;
            OnPropertyChanged();
        }
    }
    private Repeats _repeat = Repeats.None;
    public Repeats Repeat
    {
        get => _repeat;
        set
        {
            _repeat = value;
            OnPropertyChanged();
        }
    }

    public void AddEvent()
    {
        if (string.IsNullOrWhiteSpace(NewEventName)) return;

        var newEvent = new Event()
        {
            Name = NewEventName,
            Description = NewDescription,
            Due = NewDue,
            Repeats = Repeat,
        };

        Events.Add(newEvent);

        NewEventName = string.Empty;
        NewDescription = string.Empty;
        NewDue = DateTime.Today;
        Repeat = Repeats.None;

        OnEventAdded?.Invoke();
    }
}