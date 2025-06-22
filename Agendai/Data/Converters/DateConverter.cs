using System;
using System.Globalization;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;

public class DateConverter : IValueConverter
{
	public object Convert(
		object?     value,
		Type        targetType,
		object?     parameter,
		CultureInfo culture
	)
	{
		return value is not DateTime date ? string.Empty : date.ToString("dd/MM/yyyy");
	}

	public object ConvertBack(
		object?     value,
		Type        targetType,
		object?     parameter,
		CultureInfo culture
	)
	{
		return value is not string str
				? DateTime.Now
				: DateTime.ParseExact(str, "dd/MM/yyyy", CultureInfo.InvariantCulture);
	}
}
