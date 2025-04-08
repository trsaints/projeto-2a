using Agendai.Models;
using System;
using System.Collections.ObjectModel;

namespace Agendai.Services.Views
{
    public static class WeekViewService
    {
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
    }
}