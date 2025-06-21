using Agendai.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace Agendai.Services.Views
{
	public static class DayViewService
	{
		public static void GenerateDayView(
			ObservableCollection<DayRow>                     rows,
			string[]                                         hours,
			Dictionary<string, ObservableCollection<object>> dayMap,
			bool                                             showData
		)
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

		public static Dictionary<string, ObservableCollection<object>>
				MapDayItemsFrom(
					ObservableCollection<Event> events,
					ObservableCollection<Todo>  todos,
					DateTime                    selectedDay,
					string[]?                   selectedListNames
				)
		{
			var map = new Dictionary<string, ObservableCollection<object>>();

			var filteredTodos =
					(selectedListNames == null || selectedListNames.Length == 0)
							? todos
							: todos.Where(
								t => selectedListNames.Contains(t.ListName)
							);

			var filteredEvents =
					(selectedListNames == null || selectedListNames.Length == 0)
							? events
							: events.Where(
								e => selectedListNames.Contains(e.AgendaName)
							);

			foreach (var ev in filteredEvents)
			{
				if (ev.Due.Date == selectedDay.Date)
				{
					var hourKey = ev.Due.ToString("HH:mm");
					if (!map.ContainsKey(hourKey))
						map[hourKey] = new ObservableCollection<object>();

					map[hourKey].Add(ev);
				}
			}

			foreach (var todo in filteredTodos)
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
	}
}
