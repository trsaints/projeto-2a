using Avalonia.Data.Converters;
using Agendai.Data.Models;
using System;
using System.Globalization;

namespace Agendai.Data.Converters;
public class StatusToPuladoSuffixConverter : IValueConverter
{
    public static readonly StatusToPuladoSuffixConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is TodoStatus status && status == TodoStatus.Skipped
            ? " (pulado)"
            : string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}