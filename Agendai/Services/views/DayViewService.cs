using Agendai.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Agendai.Services.Views
{
    public static class DayViewService
    {
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

                    map[hourKey].Add(ev.Name);
                }
            }

            foreach (var todo in todos)
            {
                if (todo.Due.Date == selectedDay.Date)
                {
                    var hourKey = todo.Due.ToString("HH:mm");
                    if (!map.ContainsKey(hourKey))
                        map[hourKey] = new ObservableCollection<string>();

                    map[hourKey].Add(todo.Name);
                }
            }

            return map;
        }
    }
}
