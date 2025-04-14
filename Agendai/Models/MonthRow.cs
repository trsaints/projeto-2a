namespace Agendai.Models
{
    public class MonthRow
    {
        public DayCell Sunday { get; set; } = new();
        public DayCell Monday { get; set; } = new();
        public DayCell Tuesday { get; set; } = new();
        public DayCell Wednesday { get; set; } = new();
        public DayCell Thursday { get; set; } = new();
        public DayCell Friday { get; set; } = new();
        public DayCell Saturday { get; set; } = new();

        public DayCell this[int index]
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
                _ => new DayCell()
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
}