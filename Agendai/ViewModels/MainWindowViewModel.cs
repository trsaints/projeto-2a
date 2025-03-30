using System;
using Agendai.Data;
using ReactiveUI;


namespace Agendai.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
	public MainWindowViewModel()
	{
		MessageBus.Current.Listen<NavigateMessage>()
		          .Subscribe(msg => CurrentViewModel = msg.ViewModel);
		
		CurrentViewModel = new HomeWindowViewModel();
	}

	private ViewModelBase _currentViewModel;

	public ViewModelBase CurrentViewModel
	{
		get => _currentViewModel;
		set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
	}
}
