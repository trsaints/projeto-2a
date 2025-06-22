using Agendai.Data.Abstractions;
using Agendai.Data.Models;


namespace Agendai.Services.Interfaces;

public interface IRecurringScheduler<T> where T : Recurrence
{
	public RecurrenceOccurrence<T>? GetNext();
}
