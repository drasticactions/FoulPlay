using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PlaystationApp.Tools
{
    public class OnlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var colorValue = (string) value;
            if (colorValue.Equals("online"))
            {
                return new SolidColorBrush(Colors.Blue);
            }
            return colorValue.Equals("offline") ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Yellow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}