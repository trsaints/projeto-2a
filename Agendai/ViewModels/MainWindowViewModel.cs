using ReactiveUI;


namespace Agendai.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        CurrentViewModel = new HomeWindowViewModel();
    }
    
    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }
    
    public void Navigate(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
    }
}
