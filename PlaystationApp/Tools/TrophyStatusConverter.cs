using System;
using System.Globalization;
using System.Windows.Data;
using PlaystationApp.Resources;

namespace PlaystationApp.Tools
{
    public class TrophyStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return (bool) value ? AppResources.TrophyEarned : AppResources.TrophyNotEarned;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}