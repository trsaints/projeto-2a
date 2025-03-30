using System.Reactive;
using Agendai.Data;
using ReactiveUI;


namespace Agendai.ViewModels;


public class HomeWindowViewModel : ViewModelBase
{
	public HomeWindowViewModel()
	{
		OpenAgendaCommand = ReactiveCommand.Create(() =>
		{
			MessageBus.Current.SendMessage(new NavigateMessage(new AgendaWindowViewModel()));
		});	
	}
	
	public ReactiveCommand<Unit, Unit> OpenAgendaCommand { get; }
}
