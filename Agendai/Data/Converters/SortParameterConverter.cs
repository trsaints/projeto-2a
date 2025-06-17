// Agendai/Data/Converters/SortParameterConverter.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Agendai.Data.Converters;

public class SortParameterConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values?.ToArray();
    }
}