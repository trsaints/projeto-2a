using Agendai.Data.Models;

public class WeekRow
{
    public string Hour { get; set; }
    public DayCell Sunday { get; set; } = new();
    public DayCell Monday { get; set; } = new();
    public DayCell Tuesday { get; set; } = new();
    public DayCell Wednesday { get; set; } = new();
    public DayCell Thursday { get; set; } = new();
    public DayCell Friday { get; set; } = new();
    public DayCell Saturday { get; set; } = new();

    public DayCell this[string day]
    {
        get => day switch
        {
            "Sunday" => Sunday,
            "Monday" => Monday,
            "Tuesday" => Tuesday,
            "Wednesday" => Wednesday,
            "Thursday" => Thursday,
            "Friday" => Friday,
            "Saturday" => Saturday,
            _ => new DayCell()
        };
        set
        {
            switch (day)
            {
                case "Sunday": Sunday = value; break;
                case "Monday": Monday = value; break;
                case "Tuesday": Tuesday = value; break;
                case "Wednesday": Wednesday = value; break;
                case "Thursday": Thursday = value; break;
                case "Friday": Friday = value; break;
                case "Saturday": Saturday = value; break;
            }
        }
    }
}