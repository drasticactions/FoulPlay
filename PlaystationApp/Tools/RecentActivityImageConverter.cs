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
    public class RecentActivityImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var item = (RecentActivityEntity.Feed) value;
            switch (item.StoryType)
            {
                case "FRIENDED":
                    var target = item.Targets.FirstOrDefault(o => o.Type.Equals("ONLINE_ID"));
                    return target != null ? target.ImageUrl : null;
                default:
                    return item.SmallImageUrl;
            }
            //return item.SmallImageUrl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
