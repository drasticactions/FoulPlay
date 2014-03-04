using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Tools
{
    public class TrophyEarnedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TrophyDetailEntity.Trophy;
            if (item == null) return Visibility.Collapsed;
            if (item.ComparedUser != null)
            {
                return item.ComparedUser.Earned ? Visibility.Visible : Visibility.Collapsed;
            }
            if (item.FromUser != null)
            {
                return item.FromUser.Earned ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
