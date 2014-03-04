using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Tools
{
    public class PersonalNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as UserEntity;
            if (item == null) return string.Empty;
            if (item.personalDetail != null)
            {
                return !string.IsNullOrEmpty(item.personalDetail.FirstName) && !string.IsNullOrEmpty(item.personalDetail.LastName) ? string.Format("{0} {1} ({2})", item.personalDetail.FirstName, item.personalDetail.LastName, item.OnlineId) : item.OnlineId;
            }
            return item.OnlineId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
