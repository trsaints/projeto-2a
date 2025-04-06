namespace Agendai.Models
{
    public class WeekRow
    {
        public string Hour { get; set; }
        public string Sunday { get; set; } = "";
        public string Monday { get; set; } = "";
        public string Tuesday { get; set; } = "";
        public string Wednesday { get; set; } = "";
        public string Thursday { get; set; } = "";
        public string Friday { get; set; } = "";
        public string Saturday { get; set; } = "";

        public string this[string day]
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
                _ => string.Empty
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
}