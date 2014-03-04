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
    public class PersonalIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as UserEntity;
            if (item == null) return string.Empty;
            if (item.personalDetail != null)
            {
                return !string.IsNullOrEmpty(item.personalDetail.ProfilePictureUrl) ? item.personalDetail.ProfilePictureUrl : item.AvatarUrl;
            }
            return item.AvatarUrl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
