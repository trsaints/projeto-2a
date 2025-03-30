using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
    private ICommand? _openAgendaCommand;

    public ICommand OpenAgendaCommand => _openAgendaCommand ??= new RelayCommand(OpenAgenda);

    private void OpenAgenda()
    {
        MainViewModel?.NavigateToAgenda();
    }
}
