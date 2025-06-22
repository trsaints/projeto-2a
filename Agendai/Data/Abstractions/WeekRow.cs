namespace Agendai.Data.Abstractions;

public class WeekRow
{
	public string? Hour      { get; set; }
	public DayCell Sunday    { get; private set; } = new();
	public DayCell Monday    { get; private set; } = new();
	public DayCell Tuesday   { get; private set; } = new();
	public DayCell Wednesday { get; private set; } = new();
	public DayCell Thursday  { get; private set; } = new();
	public DayCell Friday    { get; private set; } = new();
	public DayCell Saturday  { get; private set; } = new();

	public DayCell this[string day]
	{
		set
		{
			switch (day)
			{
				case "Sunday":    Sunday    = value; break;
				case "Monday":    Monday    = value; break;
				case "Tuesday":   Tuesday   = value; break;
				case "Wednesday": Wednesday = value; break;
				case "Thursday":  Thursday  = value; break;
				case "Friday":    Friday    = value; break;
				case "Saturday":  Saturday  = value; break;
			}
		}
	}
}
