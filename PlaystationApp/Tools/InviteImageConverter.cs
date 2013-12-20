using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PlaystationApp.Tools
{
    public class InviteImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var visibility = (Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"];
            var url = visibility == Visibility.Visible ? "/Assets/appbar.hardware.headset.dark.png" : "/Assets/appbar.hardware.headset.light.png";
            return string.IsNullOrEmpty((string)value) ? new Uri(url, UriKind.RelativeOrAbsolute) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
