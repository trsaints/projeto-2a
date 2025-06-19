using Agendai.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Agendai.Services.Views
{
    public static class MonthViewService
    {
        public static void GenerateMonthView(
            ObservableCollection<MonthRow> rows,
            IEnumerable<Event> events,
            IEnumerable<Todo> todos,
            DateTime referenceDate,
            bool showData,
            string[]? selectedListNames,
            string? searchText
        )
        {
            rows.Clear();

            int daysInMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
            DateTime firstDay = new(referenceDate.Year, referenceDate.Month, 1);
            int startOffset = (int)firstDay.DayOfWeek;

            var filteredEvents = (selectedListNames == null || selectedListNames.Length == 0)
                ? events
                : events.Where(e => selectedListNames.Contains(e.AgendaName));

            var filteredTodos = (selectedListNames == null || selectedListNames.Length == 0)
                ? todos
                : todos.Where(t => selectedListNames.Contains(t.ListName));

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var normalized = searchText.Trim().ToLower();

                filteredEvents = filteredEvents.Where(e => e.Name != null && e.Name.ToLower().Contains(normalized));
                filteredTodos = filteredTodos.Where(t => t.Name != null && t.Name.ToLower().Contains(normalized));
            }

            var eventMap = filteredEvents
                .Where(e => e.Due.Month == referenceDate.Month && e.Due.Year == referenceDate.Year)
                .GroupBy(e => e.Due.Day)
                .ToDictionary(g => g.Key, g => g.ToList());

            var todoMap = filteredTodos
                .Where(t => t.Due.Month == referenceDate.Month && t.Due.Year == referenceDate.Year)
                .GroupBy(t => t.Due.Day)
                .ToDictionary(g => g.Key, g => g.ToList());

            int currentDay = 1;
            var totalSlots = daysInMonth + startOffset;
            var totalWeeks = (int)Math.Ceiling(totalSlots / 7.0);

            for (int w = 0; w < totalWeeks; w++)
            {
                var row = new MonthRow();
                for (int d = 0; d < 7; d++)
                {
                    int slot = w * 7 + d;
                    if (slot >= startOffset && currentDay <= daysInMonth)
                    {
                        var cell = new DayCell
                        {
                            DayNumber = currentDay,
                            Items = []
                        };

                        if (showData && eventMap.TryGetValue(currentDay, out var evts))
                            foreach (var evt in evts)
                                cell.Items.Add(evt);

                        if (showData && todoMap.TryGetValue(currentDay, out var tds))
                            foreach (var todo in tds)
                                cell.Items.Add(todo);

                        row[d] = cell;
                        currentDay++;
                    }
                    else
                    {
                        row[d] = new DayCell();
                    }
                }
                rows.Add(row);
            }
        }
    }
}
