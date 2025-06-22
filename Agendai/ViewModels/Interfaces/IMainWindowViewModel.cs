namespace Agendai.ViewModels.Interfaces;

public interface IMainWindowViewModel
{
	public ViewModelBase? CurrentViewModel { get; set; }

	public void NavigateToHome();
	public void NavigateToAgenda();
	public void NavigateToTodo();
	public void NavigateToPomodoro();
}
