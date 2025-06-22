using System;
using System.Globalization;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;

public class DateTimeToDateTimeOffsetConverter : IValueConverter
{
	public object? Convert(
		object?     value,
		Type        targetType,
		object?     parameter,
		CultureInfo culture
	)
	{
		if (value is DateTime dateTime) { return new DateTimeOffset(dateTime); }

		return null;
	}

	public object ConvertBack(
		object?     value,
		Type        targetType,
		object?     parameter,
		CultureInfo culture
	)
	{
		if (value is DateTimeOffset dto) { return dto.DateTime; }

		return DateTime.Now;
	}
}
