using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels;

public class AgendaWindowViewModel : ViewModelBase
{
    public MainWindowViewModel? MainViewModel { get; set; }

    private ICommand? _returnHomeCommand;

    public ICommand ReturnHomeCommand => _returnHomeCommand ??= new RelayCommand(ReturnHome);

    private void ReturnHome()
    {
        MainViewModel?.NavigateToHome();
    }

    public string Title { get; set; } = "Agenda";
}
