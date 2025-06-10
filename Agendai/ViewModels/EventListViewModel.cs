using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Agendai.Data;
using Agendai.Data.Models;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels
{
    public class EventListViewModel : ViewModelBase
    {
        public Action? OnEventAddedOrUpdated { get; set; }

        private Event? _currentEvent;

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            private set => SetProperty(ref _canSave, value);
        }

        public TodoWindowViewModel TodoWindowVm { get; }
        

        public EventListViewModel(TodoWindowViewModel todoWindowVm = null)
        {
            SelectTarefaCommand = new RelayCommand(() => OpenAddEvent = true);
            AddEventCommand = new RelayCommand(AddOrUpdateEvent, () => CanSave);
            OnEventAddedOrUpdated      = () => { OpenAddEvent = false; };
            CancelCommand = new RelayCommand(() =>
            {
                OpenAddEvent = false;
                NewEventName = string.Empty;
                NewDescription = string.Empty;
                NewDue = DateTime.Today;
                Repeat = Repeats.None;
                _currentEvent = null;
                UpdateCanSave();
            });


            Events = new ObservableCollection<Event>
            {
                new Event(1, "Conferência da Pamonha")
                {
                    Description = "Conferência de pamonhas",
                    Due = new DateTime(2025, 4, 6, 8, 0, 0),
                    Repeats = Repeats.Anually,
                    AgendaName = "Conferências"
                },
                new Event(2, "Feira da Foda")
                {
                    Description = "Conferência do Agro",
                    Due = new DateTime(2025, 4, 5, 14, 0, 0),
                    Repeats = Repeats.None,
                    AgendaName = "Conferências"
                },
                new Event(3, "Festa do Peão")
                {
                    Description = "Festa do Peão de Barretos",
                    Due = new DateTime(2025, 8, 20, 12, 0, 0),
                    Repeats = Repeats.Monthly,
                    AgendaName = "Festas"
                },
                new Event(4, "Feriado")
                {
                    Description = "Feriado de alguma coisa",
                    Due = new DateTime(2025, 4, 18, 22, 0, 0),
                    Repeats = Repeats.Monthly,
                    AgendaName = "Feriados"
                }
            };

            NewDue = DateTime.Today;
            NewEventName = "";
            NewDescription = "";
            Repeat = Repeats.None;

            _agendaNames = new ObservableCollection<string>(
                Events
                    .Select(e => e.AgendaName)
                    .OfType<string>()
                    .Distinct());

            if (todoWindowVm != null)
            {
                TodoWindowVm = todoWindowVm;
            }
            
            UpdateCanSave();
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
        
        private ObservableCollection<string> _agendaNames;

        public ObservableCollection<string> AgendaNames
        {
            get => _agendaNames;
            set => SetProperty(ref _agendaNames, value);
        }

        private bool _openAddEvent;
        public bool OpenAddEvent
        {
            get => _openAddEvent;
            set => SetProperty(ref _openAddEvent, value);
        }

        public ICommand AddEventCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectTarefaCommand { get; }

        private string _newEventName;
        public string NewEventName
        {
            get => _newEventName;
            set
            {
                if (SetProperty(ref _newEventName, value))
                    UpdateCanSave();
            }
        }

        private DateTime _newDue;
        public DateTime NewDue
        {
            get => _newDue;
            set
            {
                if (SetProperty(ref _newDue, value))
                    UpdateCanSave();
            }
        }

        private string _newDescription;
        public string NewDescription
        {
            get => _newDescription;
            set
            {
                if (SetProperty(ref _newDescription, value))
                    UpdateCanSave();
            }
        }

        private string _agendaName;

        public string AgendaName
        {
            get => _agendaName;
            set
            {
                if (SetProperty(ref _newDescription, value))
                    UpdateCanSave();
            }
        }

        private Repeats _repeat = Repeats.None;
        public Repeats Repeat
        {
            get => _repeat;
            set
            {
                if (SetProperty(ref _repeat, value))
                    UpdateCanSave();
            }
        }

        public IEnumerable<EventsByAgenda> EventsByAgendaName
        {
            get
            {
                return AgendaNames.Select(
                    name => new EventsByAgenda
                    {
                        AgendaName = name,
                        Events = new ObservableCollection<Event>(
                            Events.Where(e => e.AgendaName == name))
                    });
            }
        }
        
        private Event? _selectedEvent;
        public Event? SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                if (_selectedEvent != value)
                {
                    _selectedEvent = value;
                    OnPropertyChanged(nameof(SelectedEvent));
                    UpdateTodosForSelectedEvent();
                }
            }
        }
        
        private ObservableCollection<Todo> _todosForSelectedEvent = new();
        public ObservableCollection<Todo> TodosForSelectedEvent
        {
            get => _todosForSelectedEvent;
            set
            {
                _todosForSelectedEvent = value;
                OnPropertyChanged(nameof(TodosForSelectedEvent));
            }
        }

        private void UpdateTodosForSelectedEvent()
        {
            if (SelectedEvent != null)
                TodosForSelectedEvent = new ObservableCollection<Todo>(SelectedEvent.Todos ?? Enumerable.Empty<Todo>());
            else
                TodosForSelectedEvent = new ObservableCollection<Todo>();
        }

        public void LoadEvent(Event? ev)
        {
            _currentEvent = ev;

            NewEventName = ev?.Name ?? "";
            NewDescription = ev?.Description ?? "";
            NewDue = ev?.Due ?? DateTime.Today;
            Repeat = ev?.Repeats ?? Repeats.None;
            AgendaName = ev?.AgendaName ?? "";

            UpdateCanSave();
        }

        private void UpdateCanSave()
        {
            CanSave =
                !string.IsNullOrWhiteSpace(NewEventName) &&
                (
                    _currentEvent == null ||
                    NewEventName != _currentEvent.Name ||
                    NewDescription != _currentEvent.Description ||
                    NewDue != _currentEvent.Due ||
                    Repeat != _currentEvent.Repeats ||
                    AgendaName != _currentEvent.AgendaName
                );

            ((RelayCommand)AddEventCommand).NotifyCanExecuteChanged();
        }

        public void AddOrUpdateEvent()
        {
            if (!CanSave) return;

            if (_currentEvent == null)
            {
                var newEvent = new Event(Convert.ToUInt32(Events.Count + 1), NewEventName)
                {
                    Description = NewDescription,
                    Due = NewDue,
                    Repeats = Repeat,
                    AgendaName = AgendaName,
                    Todos = new List<Todo>()
                };

                if (TodoWindowVm.SelectedTodo != null)
                {
                    newEvent.Todos.Add(TodoWindowVm.SelectedTodo);
                }
                else if (!string.IsNullOrWhiteSpace(TodoWindowVm?.NewTaskName))
                {
                    var newTodo = TodoWindowVm.AddTodo(newEvent);
                    if (newTodo != null)
                    {
                        newEvent.Todos.Add(newTodo);
                    }
                }

                Events.Add(newEvent);
            }
            else
            {
                _currentEvent.Name = NewEventName;
                _currentEvent.Description = NewDescription;
                _currentEvent.Due = NewDue;
                _currentEvent.Repeats = Repeat;
                _currentEvent.AgendaName = AgendaName;

                if (_currentEvent.Todos == null)
                    _currentEvent.Todos = new List<Todo>();

                if (TodoWindowVm.SelectedTodo != null)
                {
                    if (!_currentEvent.Todos.Contains(TodoWindowVm.SelectedTodo))
                    {
                        _currentEvent.Todos.Add(TodoWindowVm.SelectedTodo);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(TodoWindowVm?.NewTaskName))
                {
                    var newTodo = TodoWindowVm.AddTodo(_currentEvent);
                    if (newTodo != null && !_currentEvent.Todos.Contains(newTodo))
                    {
                        _currentEvent.Todos.Add(newTodo);
                    }
                }
            }

            NewEventName = string.Empty;
            NewDescription = string.Empty;
            NewDue = DateTime.Today;
            Repeat = Repeats.None;
            AgendaName = string.Empty;
            TodoWindowVm.SelectedTodo = null;
            _currentEvent = null;

            OnEventAddedOrUpdated?.Invoke();
            UpdateCanSave();
        }



    }
}
