using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;

namespace Agendai.Data.Converters;

public class ViewIndexToTemplateConverter : IValueConverter
{
    public DataTemplate? MonthTemplate { get; set; }
    public DataTemplate? WeekTemplate { get; set; }
    public DataTemplate? DayTemplate { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index switch
            {
                0 => MonthTemplate,
                1 => WeekTemplate,
                2 => DayTemplate,
                _ => null
            };
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}