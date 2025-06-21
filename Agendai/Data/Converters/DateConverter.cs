using System;
using System.Globalization;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;

public class DateConverter : IValueConverter
{
	public object? Convert(
		object?     value,
		Type        targetType,
		object?     parameter,
		CultureInfo culture
	)
	{
		if (value is not DateTime date) return "";

		return date.ToString("dd/MM/yyyy");
	}

	public object ConvertBack(
		object?     value,
		Type        targetType,
		object?     parameter,
		CultureInfo culture
	)
	{
		if (value is not string str) return DateTime.Now;

		return DateTime.ParseExact(
			str,
			"dd/MM/yyyy",
			CultureInfo.InvariantCulture
		);
	}
}
