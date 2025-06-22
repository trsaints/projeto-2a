using Agendai.Data.Models;
using Agendai.Services.Recurrency;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Agendai.Services.Views;

public static class MonthViewService
{
    public static DateTime GenerateMonthView(
        ObservableCollection<MonthRow> rows,
        IEnumerable<Event>             events,
        IEnumerable<Todo>              todos,
        DateTime                       referenceDate,
        bool                           showData,
        string[]?                      selectedListNames,
        string?                        searchText
    )
    {
        rows.Clear();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var normalized = searchText.Trim().ToLower();

            var matchedEvent = events
                               .Where(e => e.Name != null && e.Name.ToLower().Contains(normalized))
                               .OrderBy(e => e.Due)
                               .FirstOrDefault();

            var matchedTodo = todos
                              .Where(t => t.Name != null && t.Name.ToLower().Contains(normalized))
                              .OrderBy(t => t.Due)
                              .FirstOrDefault();

            DateTime? matchedDate = null;
            if (matchedEvent != null && matchedTodo != null)
                matchedDate = matchedEvent.Due < matchedTodo.Due ? matchedEvent.Due : matchedTodo.Due;
            else if (matchedEvent != null)
                matchedDate = matchedEvent.Due;
            else if (matchedTodo != null)
                matchedDate = matchedTodo.Due;

            if (matchedDate.HasValue)
                referenceDate = new DateTime(matchedDate.Value.Year, matchedDate.Value.Month, 1);
        }

        int      daysInMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
        DateTime firstDay    = new(referenceDate.Year, referenceDate.Month, 1);
        int      startOffset = (int)firstDay.DayOfWeek;

        DateTime monthStart = firstDay;
        DateTime monthEnd   = firstDay.AddMonths(1).AddDays(-1);

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

        var occurrences = new List<(DateTime due, object item)>();

        foreach (var ev in filteredEvents)
        {
            if (ev.Repeats == Repeats.None)
            {
                if (ev.Due >= monthStart && ev.Due <= monthEnd)
                    occurrences.Add((ev.Due, ev));
            }
            else
            {
                var                          scheduler = new RecurringScheduler<Event>(ev);
                RecurrenceOccurrence<Event>? occ;
                while ((occ = scheduler.GetNext()) != null)
                {
                    if (occ.Due > monthEnd) break;
                    if (occ.Due >= monthStart)
                    {
                        occurrences.Add((occ.Due, occ.Item));
                    }
                }

            }
        }

        foreach (var todo in filteredTodos)
        {
            if (todo.Repeats == Repeats.None)
            {
                if (todo.Due >= monthStart && todo.Due <= monthEnd)
                    occurrences.Add((todo.Due, todo));
            }
            else
            {
                var                         scheduler = new RecurringScheduler<Todo>(todo);
                RecurrenceOccurrence<Todo>? occ;
                while ((occ = scheduler.GetNext()) != null)
                {
                    if (occ.Due > monthEnd) break;
                    if (occ.Due >= monthStart)
                    {
                        occurrences.Add((occ.Due, occ.Item));
                    }
                }

            }
        }

        var dayMap = new Dictionary<int, List<object>>();
        foreach (var (due, item) in occurrences)
        {
            int day = due.Day;
            if (!dayMap.ContainsKey(day))
                dayMap[day] = new List<object>();
            dayMap[day].Add(item);
        }

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
                        Items     = new ObservableCollection<object>()
                    };

                    if (showData && dayMap.TryGetValue(currentDay, out var items))
                    {
                        foreach (var item in items)
                            cell.Items.Add(item);
                    }

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

        return referenceDate;
    }
}