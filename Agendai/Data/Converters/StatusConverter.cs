using System;
using System.Globalization;
using Agendai.Models;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;


public class StatusConverter : IValueConverter
{
	public object? Convert(object? value,
	                       Type targetType,
	                       object? parameter,
	                       CultureInfo culture)
	{
		if (value is not TodoStatus status) return false;

		return status switch
		{
			TodoStatus.Complete   => true,
			TodoStatus.Skipped    => true,
			TodoStatus.Incomplete => false,
			_ => throw new ArgumentOutOfRangeException(
				nameof(value),
				$"{nameof(value)} is not a valid {nameof(TodoStatus)}")
		};
	}

	public object? ConvertBack(object? value,
	                           Type targetType,
	                           object? parameter,
	                           CultureInfo culture)
	{
		if (value is not bool completed) return TodoStatus.Incomplete;

		return completed ? TodoStatus.Complete : TodoStatus.Incomplete;
	}
}
