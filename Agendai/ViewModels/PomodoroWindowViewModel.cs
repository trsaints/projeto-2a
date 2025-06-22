using Agendai.ViewModels.Interfaces;


namespace Agendai.ViewModels;

public class PomodoroWindowViewModel : ViewModelBase, IPomodorWindowViewModel
{
	public string Title { get; set; } = "Meus Turnos";
}
