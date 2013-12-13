using System;
using System.Globalization;
using System.Windows.Data;

namespace PlaystationApp.Tools
{
    public class TrophyEarnedRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var trophyEarnedRate = (string) value;
            return string.Format("{0}%", trophyEarnedRate);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}