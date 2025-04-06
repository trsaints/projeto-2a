using Agendai.ViewModels;
using Avalonia.Controls;


namespace Agendai.Views.Components.CompletedTasks;


public partial class CompletedTasks : UserControl
{
    public CompletedTasks()
    {
        InitializeComponent();
        DataContext = new TodoListViewModel();
    }
}