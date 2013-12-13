using System;
using System.Globalization;
using System.Windows.Data;
using PlaystationApp.Resources;

namespace PlaystationApp.Tools
{
    public class TrophyHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return string.IsNullOrEmpty((string) value) ? AppResources.HiddenTrophy : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}