using System;


namespace Agendai.Services.Recurrence;

public class RecurrenceOccurrence<T> where T : Data.Models.Recurrence
{
	public T        Item { get; }
	public DateTime Due  { get; }

	public RecurrenceOccurrence(T item, DateTime due)
	{
		Item = item;
		Due  = due;
	}
}
