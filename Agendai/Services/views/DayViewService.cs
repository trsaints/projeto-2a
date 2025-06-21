using Agendai.Data.Models;
using Agendai.Services.Recurrency;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Agendai.Services.Views
{
    public static class DayViewService
    {
        public static void FillDayRows(
            ObservableCollection<DayRow> rows,
            string[] hours,
            Dictionary<string, ObservableCollection<object>> dayMap,
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
                        : new ObservableCollection<object>()
                };

                rows.Add(row);
            }
        }

        public static DateTime GenerateDayView(
            ObservableCollection<DayRow> rows,
            string[] hours,
            ObservableCollection<Event> events,
            ObservableCollection<Todo> todos,
            DateTime referenceDate,
            bool showData,
            string[]? selectedListNames,
            string? searchText)
        {
            var filteredTodos = (selectedListNames == null || selectedListNames.Length == 0)
                ? todos
                : todos.Where(t => selectedListNames.Contains(t.ListName));
            var filteredEvents = (selectedListNames == null || selectedListNames.Length == 0)
                ? events
                : events.Where(e => selectedListNames.Contains(e.AgendaName));

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var normalized = searchText.Trim().ToLower();

                filteredEvents = filteredEvents
                    .Where(e => !string.IsNullOrEmpty(e.Name) && e.Name.ToLower().Contains(normalized));
                filteredTodos = filteredTodos
                    .Where(t => !string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(normalized));
            }

            var minDateEvent = filteredEvents.Any() ? filteredEvents.Min(e => e.Due) : DateTime.MaxValue;
            var minDateTodo = filteredTodos.Any() ? filteredTodos.Min(t => t.Due) : DateTime.MaxValue;

            var minDate = minDateEvent < minDateTodo ? minDateEvent : minDateTodo;

            if (!string.IsNullOrWhiteSpace(searchText) && minDate != DateTime.MaxValue)
            {
                referenceDate = minDate;
            }

            var eventSchedulers = new Dictionary<ulong, RecurringScheduler<Event>>();
            foreach (var ev in filteredEvents.Where(e => e.Repeats != Repeats.None))
                eventSchedulers[ev.Id] = new RecurringScheduler<Event>(ev);

            var todoSchedulers = new Dictionary<ulong, RecurringScheduler<Todo>>();
            foreach (var todo in filteredTodos.Where(t => t.Repeats != Repeats.None))
                todoSchedulers[todo.Id] = new RecurringScheduler<Todo>(todo);

            var dayOccurrences = new List<object>();

            foreach (var ev in filteredEvents.Where(e => e.Repeats == Repeats.None && e.Due.Date == referenceDate.Date))
                dayOccurrences.Add(ev);

            foreach (var scheduler in eventSchedulers.Values)
            {
                dayOccurrences.AddRange(
                    scheduler.GetUpcoming()
                             .Where(o => o.Due.Date == referenceDate.Date)
                             .Select(o => o.Item));
            }

            foreach (var todo in filteredTodos.Where(t => t.Repeats == Repeats.None && t.Due.Date == referenceDate.Date))
                dayOccurrences.Add(todo);

            foreach (var scheduler in todoSchedulers.Values)
            {
                dayOccurrences.AddRange(
                    scheduler.GetUpcoming()
                             .Where(o => o.Due.Date == referenceDate.Date)
                             .Select(o => o.Item));
            }

            var dayMap = new Dictionary<string, ObservableCollection<object>>();

            foreach (var item in dayOccurrences)
            {
                DateTime due;
                if (item is Event ev)
                    due = ev.Due;
                else if (item is Todo td)
                    due = td.Due;
                else
                    continue;

                var hourKey = due.ToString("HH:mm");

                if (!dayMap.ContainsKey(hourKey))
                    dayMap[hourKey] = new ObservableCollection<object>();

                dayMap[hourKey].Add(item);
            }

            FillDayRows(rows, hours, dayMap, showData);

            return referenceDate;
        }

        public static Dictionary<string, ObservableCollection<object>> MapDayItemsFrom(
            IEnumerable<Event> events,
            IEnumerable<Todo> todos,
            DateTime selectedDay,
            string[]? selectedListNames)
        {
            var map = new Dictionary<string, ObservableCollection<object>>();

            foreach (var ev in events)
            {
                if (ev.Due.Date == selectedDay.Date)
                {
                    var hourKey = ev.Due.ToString("HH:mm");
                    if (!map.ContainsKey(hourKey))
                        map[hourKey] = new ObservableCollection<object>();

                    map[hourKey].Add(ev);
                }
            }

            foreach (var todo in todos)
            {
                if (todo.Due.Date == selectedDay.Date)
                {
                    var hourKey = todo.Due.ToString("HH:mm");
                    if (!map.ContainsKey(hourKey))
                        map[hourKey] = new ObservableCollection<object>();

                    map[hourKey].Add(todo);
                }
            }

            return map;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
    }
}
