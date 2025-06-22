using System.Windows.Input;


namespace Agendai.ViewModels.Interfaces;

public interface IViewModelBase
{
	public  MainWindowViewModel? MainViewModel     { get; set; }
	public  ICommand             ReturnHomeCommand { get; }
	private void                 ReturnHome()      { MainViewModel?.NavigateToHome(); }
}
