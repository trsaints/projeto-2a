using Agendai.Data.Models;
using Agendai.ViewModels.Agenda;
using Avalonia.Controls;


namespace Agendai.Views.Components.Agenda;

public partial class Agenda : UserControl
{
	public Agenda() { InitializeComponent(); }

	public void OnEventOrTodoCLicked(object? sender)
	{
		var viewModel = DataContext as AgendaWindowViewModel;

		if (sender is not Button button) return;

		switch (button.Tag)
		{
			case Event ev:
				viewModel?.EditEvent(ev);

				break;

			case Todo todo:
				viewModel?.EditTodo(todo);

				break;
		}
	}
}
