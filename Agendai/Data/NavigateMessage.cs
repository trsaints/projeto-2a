using Agendai.ViewModels;
using ReactiveUI;


namespace Agendai.Data;


public class NavigateMessage
{
	public NavigateMessage(ViewModelBase viewModel)
	{
		ViewModel = viewModel;
	}
	
	public ViewModelBase ViewModel { get; }
}
