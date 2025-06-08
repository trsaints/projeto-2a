using System;
using System.Collections.ObjectModel;
using Agendai.Data.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;


public class EventListViewModel : ViewModelBase
{
	public Action? OnEventAdded { get; set; }
	public EventListViewModel()
	{
		OnEventAdded = () => { OpenAddEvent = false; };
		SelectTarefaCommand = new RelayCommand(
			() =>
			{
				OpenAddEvent = true;
			}
		);
		AddEventCommand = new RelayCommand(AddEvent);
		CancelCommand = new RelayCommand(
			() =>
			{
				OpenAddEvent = false;
			}
		);
		
		Events =
		[
			new Event(1, "Conferência da Pamonha")
			{
				Description = "Conferência de pamonhas",
				Due = new DateTime(2025, 4, 6, 8, 0, 0),
				Repeats = Repeats.Anually
			},
			new Event(2, "Feira da Foda")
			{
				Description = "Conferência do Agro",
				Due = new DateTime(2025, 4, 5, 14, 0, 0),
				Repeats = Repeats.None
			},
			new Event(3, "Festa do Peão")
			{
				Description = "Festa do Peão de Barretos",
				Due = new DateTime(2025, 8, 20, 12, 0, 0),
				Repeats = Repeats.Monthly
			},
			new Event(4, "Feriado")
			{
				Description = "Feriado de alguma coisa",
				Due = new DateTime(2025, 4, 18, 22, 0, 0),
				Repeats = Repeats.Monthly
			}
		];
	}
	
	public ObservableCollection<Event> Events { get; set; }
	public ObservableCollection<Repeats> RepeatOptions { get; } = new ObservableCollection<Repeats>
	{
            Repeats.None,
            Repeats.Daily,
            Repeats.Weekly,
            Repeats.Monthly,
            Repeats.Anually
	};
	
	private bool _openAddEvent;

	public bool OpenAddEvent
	{
		get => _openAddEvent;

		set => SetProperty(ref _openAddEvent, value);
	}
	
	public  ICommand AddEventCommand { get; }

	public ICommand CancelCommand { get; }
	
	public ICommand SelectTarefaCommand { get; }
	
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
		if (String.IsNullOrWhiteSpace(NewEventName)) return;
        
		var newEvent = new Event(Convert.ToUInt32(Events.Count + 1), NewEventName)
		{
			Description = NewDescription,
			Due = NewDue,
			Repeats = Repeat,
		};
		Events.Add(newEvent);
        
		NewEventName = string.Empty;
		NewDescription = String.Empty;
		NewDue = DateTime.Today;
		Repeat = Repeats.None;
		
		OnEventAdded?.Invoke();
	}
}