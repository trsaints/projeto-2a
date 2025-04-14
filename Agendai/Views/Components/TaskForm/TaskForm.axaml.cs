using Agendai.ViewModels;
using Avalonia.Controls;


namespace Agendai.Views.Components.TaskForm;

public partial class TaskForm : UserControl
{
    public TaskForm()
    {
        InitializeComponent();
        DataContext = new TodoWindowViewModel();
    }
}