using Agendai.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace Agendai.Views.Components.EventList;

public partial class EventList : UserControl
{
	public EventList()
	{
		InitializeComponent();
		DataContext = new EventListViewModel();
	}
}
