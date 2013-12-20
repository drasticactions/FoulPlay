using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Tools
{
    public class MaxPlayersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var session = (SessionInviteDetailEntity.Session) value;
            int members = session.Members.Count();
            return string.Format("{0}/{1}", members, session.SessionMaxUser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
