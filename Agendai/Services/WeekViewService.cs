using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;


namespace Agendai.Services;

public static class WeekViewService
{
	public static DateTime GenerateWeekView(
		ObservableCollection<WeekRow> rows,
		string[]                      hours,
		ObservableCollection<Event>   events,
		ObservableCollection<Todo>    todos,
		DateTime                      referenceDate,
		bool                          showData,
		string[]?                     selectedListNames,
		string?                       searchText
	)
	{
		rows.Clear();

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
					.Where(e => !string.IsNullOrEmpty(e.Name)
					            && e.Name.ToLower().Contains(normalized)
					);
			filteredTodos = filteredTodos
					.Where(t => !string.IsNullOrEmpty(t.Name)
					            && t.Name.ToLower().Contains(normalized)
					);

			var minDateEvent = filteredEvents.MinOrDefault(e => e.Due, DateTime.MaxValue);
			var minDateTodo  = filteredTodos.MinOrDefault(t => t.Due, DateTime.MaxValue);
			var minDate      = minDateEvent < minDateTodo ? minDateEvent : minDateTodo;

			if (minDate != DateTime.MaxValue) { referenceDate = minDate; }
		}

		var startOfWeek = referenceDate.AddDays(-(int)referenceDate.DayOfWeek);
		var endOfWeek   = startOfWeek.AddDays(6);

		var occurrenceMap = new Dictionary<DateTime, List<object>>();

		foreach (var ev in filteredEvents.Where(e => e.Repeats != Repeats.None))
		{
			var                          scheduler = new RecurringScheduler<Event>(ev);
			RecurrenceOccurrence<Event>? occ;

			while ((occ = scheduler.GetNext()) != null)
			{
				if (occ.Due.Date > endOfWeek) break;

				if (occ.Due.Date >= startOfWeek)
				{
					var key = new DateTime(
						occ.Due.Year,
						occ.Due.Month,
						occ.Due.Day,
						occ.Due.Hour,
						0,
						0
					);
					if (!occurrenceMap.ContainsKey(key))
						occurrenceMap[key] = new List<object>();
					occurrenceMap[key].Add(occ.Item);
				}
			}
		}

		foreach (var todo in filteredTodos.Where(t => t.Repeats != Repeats.None))
		{
			var                         scheduler = new RecurringScheduler<Todo>(todo);
			RecurrenceOccurrence<Todo>? occ;

			while ((occ = scheduler.GetNext()) != null)
			{
				if (occ.Due.Date > endOfWeek) break;

				if (occ.Due.Date >= startOfWeek)
				{
					var key = new DateTime(
						occ.Due.Year,
						occ.Due.Month,
						occ.Due.Day,
						occ.Due.Hour,
						0,
						0
					);
					if (!occurrenceMap.ContainsKey(key))
						occurrenceMap[key] = new List<object>();
					occurrenceMap[key].Add(occ.Item);
				}
			}
		}

		foreach (var hour in hours)
		{
			var row = new WeekRow { Hour = hour };

			for (int i = 0; i < 7; i++)
			{
				var day  = startOfWeek.AddDays(i);
				var cell = new DayCell { Items = new ObservableCollection<object>() };

				if (showData)
				{
					foreach (var e in filteredEvents.Where(e => e.Repeats == Repeats.None))
					{
						if (e.Due.Date == day.Date && e.Due.ToString("HH:00") == hour)
							cell.Items.Add(e);
					}

					foreach (var t in filteredTodos.Where(t => t.Repeats == Repeats.None))
					{
						if (t.Due.Date == day.Date && t.Due.ToString("HH:00") == hour)
							cell.Items.Add(t);
					}

					var cellKey = new DateTime(
						day.Year,
						day.Month,
						day.Day,
						TimeSpan.Parse(hour).Hours,
						0,
						0
					);

					if (occurrenceMap.TryGetValue(cellKey, out var items))
					{
						foreach (var occItem in items)
							cell.Items.Add(occItem);
					}
				}

				row[day.DayOfWeek.ToString()] = cell;
			}

			rows.Add(row);
		}

		return referenceDate;
	}

	public static TKey MinOrDefault<TSource, TKey>(
		this IEnumerable<TSource> source,
		Func<TSource, TKey>       selector,
		TKey                      defaultValue
	)
	{
		return source.Any() ? source.Min(selector) : defaultValue;
	}

	public static (int WeekNumber, DateTime StartOfWeek, DateTime EndOfWeek)
			GetWeekOfMonthRange(DateTime date)
	{
		var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
		var dayOfWeekOffset = (int)firstDayOfMonth.DayOfWeek;

		int totalDays  = (date - firstDayOfMonth).Days + dayOfWeekOffset;
		int weekNumber = (int)Math.Floor(totalDays / 7.0) + 1;

		var startOfWeek = date.Date.AddDays(-(int)date.DayOfWeek);

		if (startOfWeek.Month < date.Month)
			startOfWeek = new DateTime(date.Year, date.Month, 1);

		var endOfWeek = startOfWeek.AddDays(6);
		if (endOfWeek.Month > date.Month)
			endOfWeek = new DateTime(
				date.Year,
				date.Month,
				DateTime.DaysInMonth(date.Year, date.Month)
			);

		return (weekNumber, startOfWeek, endOfWeek);
	}
}
