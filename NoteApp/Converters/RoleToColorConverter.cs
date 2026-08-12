// Converters/RoleToColorConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NoteApp.Converters
{
    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role)
            {
                return role == "Admin"
                    ? new SolidColorBrush(Color.FromRgb(231, 76, 60)) // #E74C3C
                    : new SolidColorBrush(Color.FromRgb(52, 152, 219)); // #3498DB
            }
            return new SolidColorBrush(Color.FromRgb(149, 165, 166)); // #95A5A6
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}