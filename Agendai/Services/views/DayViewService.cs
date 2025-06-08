using Agendai.Data.Models;
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
            Dictionary<string, ObservableCollection<string>> dayMap,
            bool showData)
        {
            rows.Clear();

            foreach (var hour in hours)
            {
                var row = new DayRow
                {
                    Hour = hour,
                    Items = showData && dayMap.TryGetValue(hour, out var items)
                        ? items
                        : new ObservableCollection<string>()

                };

                rows.Add(row);
            }
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
