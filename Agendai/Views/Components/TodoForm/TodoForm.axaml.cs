using Agendai.ViewModels;
using Avalonia.Controls;


namespace Agendai.Views.Components.TodoForm;

public partial class TodoForm : UserControl
{
    public TodoForm()
    {
        InitializeComponent();
        DataContext = new TodoWindowViewModel();
    }
}