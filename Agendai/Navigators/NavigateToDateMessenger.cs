using System;

namespace Agendai.Navigators;

public class NavigateToDateMessenger
{
    public DateTime SelectedDate { get; set; }
    public NavigateToDateMessenger(DateTime selectedDate)
    {
        SelectedDate = selectedDate;
    }
}
