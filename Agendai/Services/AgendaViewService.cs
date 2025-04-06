using Agendai.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Agendai.Services
{
    public static class AgendaViewService
    {
        public static void GenerateMonthView(ObservableCollection<MonthRow> rows, IEnumerable<Event> events, IEnumerable<Todo> todos)
        {
            rows.Clear();

            var today = DateTime.Today;
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            DateTime firstDay = new DateTime(today.Year, today.Month, 1);
            int startOffset = (int)firstDay.DayOfWeek;

            var eventMap = events
                .Where(e => e.Due.Month == today.Month && e.Due.Year == today.Year)
                .GroupBy(e => e.Due.Day)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Name).ToList());

            var todoMap = todos
                .Where(t => t.Due.Month == today.Month && t.Due.Year == today.Year)
                .GroupBy(t => t.Due.Day)
                .ToDictionary(g => g.Key, g => g.Select(t => t.Name).ToList());

            int day = 1;
            int totalSlots = daysInMonth + startOffset;
            int totalWeeks = (int)Math.Ceiling(totalSlots / 7.0);

            for (int w = 0; w < totalWeeks; w++)
            {
                var row = new MonthRow();
                for (int d = 0; d < 7; d++)
                {
                    int slot = w * 7 + d;
                    if (slot >= startOffset && day <= daysInMonth)
                    {
                        var cell = new DayCell
                        {
                            DayNumber = day,
                            Items = new ObservableCollection<string>()
                        };

                        if (eventMap.TryGetValue(day, out var evts))
                        {
                            foreach (var evt in evts)
                                cell.Items.Add(evt);
                        }
                        if (todoMap.TryGetValue(day, out var tds))
                        {
                            foreach (var todo in tds)
                                cell.Items.Add(todo);
                        }
                        
                        row[d] = cell;
                        day++;
                    }
                    else
                    {
                        row[d] = new DayCell();
                    }
                }
                rows.Add(row);
            }
        }

        public static void GenerateWeekView(ObservableCollection<WeekRow> rows, string[] hours, Dictionary<(string Hour, string Day), string> events)
        {
            rows.Clear();
            foreach (var hour in hours)
            {
                var row = new WeekRow { Hour = hour };
                foreach (var day in new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" })
                {
                    row[day] = events.TryGetValue((hour, day), out var evt) ? evt : string.Empty;
                }
                rows.Add(row);
            }
        }

        public static void GenerateDayView(ObservableCollection<DayRow> rows, string[] hours, Dictionary<(string Hour, string Day), string> events)
        {
            rows.Clear();
            var today = DateTime.Today;
            var culture = new CultureInfo("pt-BR");
            string day = culture.TextInfo.ToTitleCase(
                today.ToString("dddd, d 'de' MMMM", culture)
            );


            foreach (var hour in hours)
            {
                var row = new DayRow
                {
                    Hour = hour,
                    Event = events.TryGetValue((hour, day), out var evt) ? evt : string.Empty
                };
                rows.Add(row);
            }
        }

        public static void AddSampleEvents(Dictionary<(string Hour, string Day), string> events)
        {
            events[("08:00", "Monday")] = "Team meeting";
            events[("14:00", "Wednesday")] = "Code review";
            events[("16:00", "Friday")] = "Project presentation";
        }

        public static int GetWeekOfMonth(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var firstDayOfWeek = (int)firstDay.DayOfWeek;
            var dayOfMonth = date.Day + firstDayOfWeek;
            return (int)Math.Ceiling(dayOfMonth / 7.0);
        }
    }
}
