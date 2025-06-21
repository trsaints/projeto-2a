using System;
using System.Collections.Generic;

namespace Agendai.Services.Recurrence
{
    public class RecurringOccurrenceManager<T> where T : ICloneableWithDate
    {
        private readonly Func<DateTime, DateTime> _getNextDate;
        private readonly int _maxOccurrences;

        public List<T> Upcoming { get; } = new();
        public Stack<T> Past { get; } = new();

        public RecurringOccurrenceManager(T template, int maxOccurrences, Func<DateTime, DateTime> getNextDate)
        {
            _getNextDate = getNextDate;
            _maxOccurrences = maxOccurrences;

            var currentDate = template.Due;
            for (int i = 0; i < maxOccurrences; i++)
            {
                var clone = (T)template.Clone();
                clone.Due = currentDate;
                Upcoming.Add(clone);
                currentDate = _getNextDate(currentDate);
            }
        }

        public T Advance()
        {
            if (Upcoming.Count == 0) throw new InvalidOperationException("No upcoming occurrences left!");

            var next = Upcoming[0];
            Upcoming.RemoveAt(0);
            Past.Push(next);

            var lastDate = Upcoming.Count > 0 ? Upcoming[^1].Due : next.Due;
            var newDate = _getNextDate(lastDate);
            var newClone = (T)next.Clone();
            newClone.Due = newDate;
            Upcoming.Add(newClone);

            return next;
        }

        public T Revert()
        {
            if (Past.Count == 0) throw new InvalidOperationException("No past occurrence to revert!");

            var lastPast = Past.Pop();
            Upcoming.Insert(0, lastPast);

            if (Upcoming.Count > _maxOccurrences)
                Upcoming.RemoveAt(Upcoming.Count - 1);

            return lastPast;
        }
    }
}