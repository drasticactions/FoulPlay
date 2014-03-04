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
    public class TrophyDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TrophyDetailEntity.Trophy;
            if (item == null) return Visibility.Collapsed;
            if (item.ComparedUser != null)
            {
                try
                {
                    return item.ComparedUser.EarnedDate != null ? DateTime.Parse(item.ComparedUser.EarnedDate).ToLocalTime().ToString(CultureInfo.CurrentCulture) : string.Empty;

                }
                catch (Exception)
                {
                  return string.Empty;
                }
            }
            if (item.FromUser != null)
            {
                try
                {
                    return item.FromUser.EarnedDate != null ? DateTime.Parse(item.FromUser.EarnedDate).ToLocalTime().ToString(CultureInfo.CurrentCulture) : string.Empty;

                }
                catch (Exception)
                {
                   return string.Empty;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
