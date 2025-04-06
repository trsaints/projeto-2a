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

        public static void GenerateWeekView(
            ObservableCollection<WeekRow> rows,
            string[] hours,
            ObservableCollection<Event> events,
            ObservableCollection<Todo> todos)
        {
            rows.Clear();
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
    
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
                            cell.Items.Add($"Evento: {e.Description}");
                    }

                    foreach (var t in todos)
                    {
                        if (t.Due.Date == day.Date && t.Due.ToString("HH:00") == hour)
                            cell.Items.Add($"Tarefa: {t.Description}");
                    }

                    row[day.DayOfWeek.ToString()] = cell;
                }

                rows.Add(row);
            }
        }


        public static void GenerateDayView(
            ObservableCollection<DayRow> rows,
            string[] hours,
            Dictionary<string, ObservableCollection<string>> dayMap)
        {
            rows.Clear();

            foreach (var hour in hours)
            {
                var row = new DayRow
                {
                    Hour = hour,
                    Items = dayMap.TryGetValue(hour, out var items) ? items : new ObservableCollection<string>()
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
        
        public static Dictionary<string, ObservableCollection<string>> MapDayItemsFrom(
            ObservableCollection<Event> events,
            ObservableCollection<Todo> todos,
            DateTime selectedDay)
        {
            var map = new Dictionary<string, ObservableCollection<string>>();

            foreach (var ev in events)
            {
                if (ev.Due.Date == selectedDay.Date)
                {
                    var hourKey = ev.Due.ToString("HH:mm");
                    if (!map.ContainsKey(hourKey))
                        map[hourKey] = new ObservableCollection<string>();

                    map[hourKey].Add($"Evento: {ev.Name}");
                }
            }

            foreach (var todo in todos)
            {
                if (todo.Due.Date == selectedDay.Date)
                {
                    var hourKey = todo.Due.ToString("HH:mm");
                    if (!map.ContainsKey(hourKey))
                        map[hourKey] = new ObservableCollection<string>();

                    map[hourKey].Add($"Tarefa: {todo.Name}");
                }
            }

            return map;
        }

    }
}
