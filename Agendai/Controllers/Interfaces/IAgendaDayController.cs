using System;


namespace Agendai.Controllers.Interfaces;

public interface IAgendaDayController
{
	public void GoToPreviousDay();
	public void GoToNextDay();
	public void UpdateDayFromDate(DateTime selectedDate);
	public void GoToDay(int                dayNumber);
}
