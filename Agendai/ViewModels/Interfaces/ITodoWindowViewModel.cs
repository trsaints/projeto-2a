using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;


namespace Agendai.ViewModels.Interfaces;

public interface ITodoWindowViewModel
{
	HomeWindowViewModel? HomeWindowVm { get; set; }
	EventListViewModel?  EventListVm  { get; set; }

	Action?   OnTaskAdded           { get; set; }
	ICommand? OpenPopupCommand      { get; }
	ICommand? SelectTarefaCommand   { get; }
	ICommand? AddTodoCommand        { get; }
	ICommand? CancelCommand         { get; }
	ICommand? AddTodoToEventCommand { get; }

	bool OpenAddTask { get; set; }

	ObservableCollection<Todo>          Todos            { get; set; }
	ObservableCollection<Todo>          IncompleteTodos  { get; set; }
	ObservableCollection<Todo>          TodoHistory      { get; set; }
	ObservableCollection<Todo>          IncompleteResume { get; set; }
	ObservableCollection<string>        ListNames        { get; set; }
	ObservableCollection<RepeatsOption> RepeatOptions    { get; }
	ObservableCollection<Todo?>         FreeTodos        { get; }
	IEnumerable<TodosByListName>        TodosByListName  { get; }
	IEnumerable<string>                 FreeTodosNames   { get; }

	string        NewTaskName      { get; set; }
	string        NewDescription   { get; set; }
	string        SelectedTodoName { get; set; }
	string        ListName         { get; set; }
	DateTime      NewDue           { get; set; }
	bool          IsPopupOpen      { get; set; }
	bool          HasChanges       { get; }
	RepeatsOption SelectedRepeats  { get; set; }
	Todo?         EditingTodo      { get; set; }
	Todo?         SelectedTodo     { get; set; }

	void ClearTodoForm();
}
