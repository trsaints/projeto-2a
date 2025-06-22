using System;


namespace Agendai.Messages;

public class NavigateToDateMessenger
{
	public DateTime SelectedDate { get; set; }
	public NavigateToDateMessenger(DateTime selectedDate) { SelectedDate = selectedDate; }
}
