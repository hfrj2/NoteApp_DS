// Converters/BooleanToColorConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NoteApp.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string colors)
            {
                var parts = colors.Split(',');
                if (parts.Length == 2)
                {
                    var colorStr = boolValue ? parts[0] : parts[1];
                    try
                    {
                        var color = (Color)ColorConverter.ConvertFromString(colorStr);
                        return new SolidColorBrush(color);
                    }
                    catch
                    {
                        return new SolidColorBrush(Color.FromRgb(149, 165, 166));
                    }
                }
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}