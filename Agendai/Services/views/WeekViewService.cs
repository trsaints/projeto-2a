using Agendai.Models;
using System;
using System.Collections.ObjectModel;

namespace Agendai.Services.Views
{
    public static class WeekViewService
    {
        public static void GenerateWeekView(
            ObservableCollection<WeekRow> rows,
            string[] hours,
            ObservableCollection<Event> events,
            ObservableCollection<Todo> todos,
            DateTime referenceDate)
        {
            rows.Clear();
            var startOfWeek = referenceDate.AddDays(-(int)referenceDate.DayOfWeek);

            foreach (var hour in hours)
            {
                var row = new WeekRow { Hour = hour };

                for (int i = 0; i < 7; i++)
                {
                    var day = startOfWeek.AddDays(i);
                    var cell = new DayCell
                    {
                        Items = new ObservableCollection<string>()
                    };

                    foreach (var e in events)
                    {
                        if (e.Due.Date == day.Date && e.Due.ToString("HH:00") == hour)
                            cell.Items.Add(e.Description);
                    }

                    foreach (var t in todos)
                    {
                        if (t.Due.Date == day.Date && t.Due.ToString("HH:00") == hour)
                            cell.Items.Add(t.Description);
                    }

                    row[day.DayOfWeek.ToString()] = cell;
                }

                rows.Add(row);
            }
        }
        
        public static (int WeekNumber, DateTime StartOfWeek, DateTime EndOfWeek) GetWeekOfMonthRange(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var dayOfWeekOffset = (int)firstDayOfMonth.DayOfWeek; 
            
            int totalDays = (date - firstDayOfMonth).Days + dayOfWeekOffset;
            int weekNumber = (int)Math.Floor(totalDays / 7.0) + 1;
            
            var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek);

            if (startOfWeek.Month < date.Month)
                startOfWeek = new DateTime(date.Year, date.Month, 1);

            var endOfWeek = startOfWeek.AddDays(6);
            if (endOfWeek.Month > date.Month)
                endOfWeek = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            return (weekNumber, startOfWeek, endOfWeek);
        }
    }
}