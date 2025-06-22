using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;
using Avalonia.Media;


namespace Agendai.ViewModels.Interfaces;

public interface IEventListViewModel
{
	public ObservableCollection<Event>         Events                { get; set; }
	public ObservableCollection<RepeatsOption> RepeatOptions         { get; }
	public ObservableCollection<string>        AgendaNames           { get; set; }
	public ObservableCollection<Todo?>         TodosForSelectedEvent { get; set; }
	public IEnumerable<EventsByAgenda>         EventsByAgendaName    { get; }

	public bool                 IsAddTodoPopupOpen { get; set; }
	public bool                 OpenAddEvent       { get; set; }
	public bool                 CanSave            { get; }
	public string               NewEventName       { get; set; }
	public DateTime             NewDue             { get; set; }
	public string               NewDescription     { get; set; }
	public string               AgendaName         { get; set; }
	public RepeatsOption        Repeat             { get; set; }
	public Event?               SelectedEvent      { get; }
	public bool                 HasRelatedTodos    { get; set; }
	public TodoWindowViewModel? TodoWindowVm       { get; }
	public Color?               NewColor           { get; set; }

	public Action?  OnEventAddedOrUpdated { get; set; }
	public ICommand AddEventCommand       { get; }
	public ICommand CancelCommand         { get; }
	public ICommand OpenPopupCommand      { get; }
	public ICommand ClosePopupCommand     { get; }

	public void LoadEvent(Event? ev);
	public void UpdateCanSave();
	public void NotifyTodosForSelectedEventChanged();
	public void RemoveTodoFromEvent(Todo? todo);
}
