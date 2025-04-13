using Agendai.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agendai.Views.Components.TodoList;

public partial class AddTask : UserControl
{
    public AddTask()
    {
        InitializeComponent();
        DataContext = new TodoWindowViewModel();
    }
}