using Avalonia.Data.Converters;
using Avalonia.Media;
using Agendai.Data.Models;
using System;
using System.Globalization;

namespace Agendai.Data.Converters
{
    public class StatusToStrikethroughConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is TodoStatus status && status == TodoStatus.Skipped)
                return TextDecorations.Strikethrough;

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}