using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PlaystationApp.Tools
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateString = value as string;
            if (string.IsNullOrEmpty(dateString)) return value;
            try
            {
                DateTime date = DateTime.Parse(dateString);
                return date.ToLocalTime().ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
