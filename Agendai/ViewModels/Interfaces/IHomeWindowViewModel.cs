using System.Windows.Input;


namespace Agendai.ViewModels.Interfaces;

public interface IHomeWindowViewModel
{
	public bool IsPopupOpen            { get; set; }
	public bool IsEventListsAbleToView { get; set; }
	public bool IsTodoListsAbleToView  { get; set; }
	public bool IsAgendaWindow         { get; set; }
	public bool IsTodoWindow           { get; set; }
	public bool IsPomodoroWindow       { set; }

	public TodoWindowViewModel? TodoWindowVm { get; set; }
	public EventListViewModel?  EventListVm  { get; set; }

	public string   EventListsVisibilityText { get; }
	public string   TodoListsVisibilityText  { get; }
	public ICommand OpenPopupCommand         { get; }
	public ICommand OpenTodoFormCommand      { get; }
	public ICommand OpenEventFormCommand     { get; }
	public ICommand OpenAgendaCommand        { get; }
	public ICommand OpenTodoCommand          { get; }
	public ICommand OpenPomodoroCommand      { get; }
}
