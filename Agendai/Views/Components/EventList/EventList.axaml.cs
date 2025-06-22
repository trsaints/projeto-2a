using Agendai.ViewModels;
using Avalonia.Controls;


namespace Agendai.Views.Components.EventList;

public partial class EventList : UserControl
{
	public EventList()
	{
		InitializeComponent();
		DataContext = new EventListViewModel();
	}
}
