namespace Agendai.Data.Abstractions;

public class MonthRow
{
	public DayCell Sunday    { get; private set; } = new();
	public DayCell Monday    { get; private set; } = new();
	public DayCell Tuesday   { get; private set; } = new();
	public DayCell Wednesday { get; private set; } = new();
	public DayCell Thursday  { get; private set; } = new();
	public DayCell Friday    { get; private set; } = new();
	public DayCell Saturday  { get; private set; } = new();

	public DayCell this[int index]
	{
		set
		{
			switch (index)
			{
				case 0: Sunday    = value; break;
				case 1: Monday    = value; break;
				case 2: Tuesday   = value; break;
				case 3: Wednesday = value; break;
				case 4: Thursday  = value; break;
				case 5: Friday    = value; break;
				case 6: Saturday  = value; break;
			}
		}
	}
}
