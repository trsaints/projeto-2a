using Agendai.ViewModels;
using Avalonia.Controls;


namespace Agendai.Views.Windows.TodoWindow;

public partial class TodoWindow : UserControl
{
	public TodoWindow()
	{
		InitializeComponent();
		
		DataContext = new TodoWindowViewModel();
	}
}
