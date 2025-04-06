namespace Agendai.Models;

public class MonthRow
{
    public string Sunday { get; set; } = "";
    public string Monday { get; set; } = "";
    public string Tuesday { get; set; } = "";
    public string Wednesday { get; set; } = "";
    public string Thursday { get; set; } = "";
    public string Friday { get; set; } = "";
    public string Saturday { get; set; } = "";

    public string this[int index]
    {
        get => index switch
        {
            0 => Sunday,
            1 => Monday,
            2 => Tuesday,
            3 => Wednesday,
            4 => Thursday,
            5 => Friday,
            6 => Saturday,
            _ => ""
        };
        set
        {
            switch (index)
            {
                case 0: Sunday = value; break;
                case 1: Monday = value; break;
                case 2: Tuesday = value; break;
                case 3: Wednesday = value; break;
                case 4: Thursday = value; break;
                case 5: Friday = value; break;
                case 6: Saturday = value; break;
            }
        }
    }
}
