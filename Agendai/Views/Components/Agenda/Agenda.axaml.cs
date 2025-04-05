using Agendai.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;


namespace Agendai.Views.Components.Agenda;

public partial class Agenda : UserControl
{
    public Agenda()
    {
        InitializeComponent();
        DataContext = new AgendaWindowViewModel();
    }
}