using System;
using Agendai.Data.Models;


namespace Agendai.Data.Abstractions;

public class RecurrenceOccurrence<T> where T : Recurrence
{
	public T        Item { get; }
	public DateTime Due  { get; }

	public RecurrenceOccurrence(T item, DateTime due)
	{
		Item = item;
		Due  = due;
	}
}
