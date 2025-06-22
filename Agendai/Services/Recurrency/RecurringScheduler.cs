using System;
using System.Collections.Generic;
using System.Linq;
using Agendai.Data.Models;

namespace Agendai.Services.Recurrency
{
    public class RecurringScheduler<T> where T : Recurrence
    {
        private readonly T _template;
        private readonly int _limit;
        private readonly Queue<RecurrenceOccurrence<T>> _upcoming = new();
        private readonly Stack<RecurrenceOccurrence<T>> _past = new();

        public RecurringScheduler(T template, int limit = 32)
        {
            _template = template;
            _limit = limit;
            FillOccurrences(template.Due);
        }

        public RecurrenceOccurrence<T>? GetNext()
        {
            if (_upcoming.Count == 0) return null;

            var next = _upcoming.Dequeue();
            _past.Push(next);

            FillOccurrences(next.Due);
            return next;
        }

        private void FillOccurrences(DateTime from)
        {
            var last = _upcoming.Count > 0 ? _upcoming.Last().Due : _template.Repeats switch
            {
                Repeats.Daily => from.AddDays(-1),
                Repeats.Weekly => from.AddDays(-7),
                Repeats.Monthly => from.AddMonths(-1),
                Repeats.Anually => from.AddYears(-1),
                _ => from.AddDays(-1)
            };

            while (_upcoming.Count < _limit)
            {
                last = GetNextOccurrence(last, _template.Repeats);
                _upcoming.Enqueue(new RecurrenceOccurrence<T>(_template, last));
            }
        }

        private DateTime GetNextOccurrence(DateTime date, Repeats repeats) => repeats switch
        {
            Repeats.Daily => date.AddDays(1),
            Repeats.Weekly => date.AddDays(7),
            Repeats.Monthly => date.AddMonths(1),
            Repeats.Anually => date.AddYears(1),
            _ => date
        };
    }
}
