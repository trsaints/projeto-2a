using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace  Agendai.Data.Converters
{
    public class EventNameDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
    
            string str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return "";
    
            return $"Evento associado: {str}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}



