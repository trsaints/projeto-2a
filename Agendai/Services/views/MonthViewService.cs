using Agendai.Models;
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
            bool showData)
        {
            rows.Clear();

            int daysInMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
            DateTime firstDay = new DateTime(referenceDate.Year, referenceDate.Month, 1);
            int startOffset = (int)firstDay.DayOfWeek;

            var eventMap = events
                .Where(e => e.Due.Month == referenceDate.Month && e.Due.Year == referenceDate.Year)
                .GroupBy(e => e.Due.Day)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Name).ToList());

            var todoMap = todos
                .Where(t => t.Due.Month == referenceDate.Month && t.Due.Year == referenceDate.Year)
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

                        if (showData && eventMap.TryGetValue(day, out var evts))
                            foreach (var evt in evts)
                                cell.Items.Add(evt);

                        if (showData && todoMap.TryGetValue(day, out var tds))
                            foreach (var todo in tds)
                                cell.Items.Add(todo);

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

    }
}
