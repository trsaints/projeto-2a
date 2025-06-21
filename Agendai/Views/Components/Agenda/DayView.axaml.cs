using Agendai.ViewModels;
using Agendai.ViewModels.Agenda;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;


namespace Agendai.Views.Components.Agenda;

public partial class DayView : UserControl
{
	public DayView() { InitializeComponent(); }

	private void OnPreviousDayClicked(object? sender, RoutedEventArgs e)
	{
		if (DataContext is AgendaWindowViewModel vm)
			vm.GoToPreviousDay();
	}

	private void OnNextDayClicked(object? sender, RoutedEventArgs e)
	{
		if (DataContext is AgendaWindowViewModel vm)
			vm.GoToNextDay();
	}

	private void ForwardClickToParent(object? sender, RoutedEventArgs e)
	{
		var parentAgenda = this.FindAncestorOfType<Agenda>();
		parentAgenda?.OnEventOrTodoCLicked(sender, e);
	}
}
