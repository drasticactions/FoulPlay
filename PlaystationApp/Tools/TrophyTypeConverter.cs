using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PlaystationApp.Tools
{
    public class TrophyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var trophyType = (string) value;
            switch (trophyType)
            {
                case "platinum":
                    return new BitmapImage(new Uri("/Assets/Trophy-icon-plat.png", UriKind.RelativeOrAbsolute));
                case "gold":
                    return new BitmapImage(new Uri("/Assets/Trophy-icon-Gold.png", UriKind.RelativeOrAbsolute));
                case "silver":
                    return new BitmapImage(new Uri("/Assets/Trophy-icon-Silver.png", UriKind.RelativeOrAbsolute));
                case "bronze":
                    return new BitmapImage(new Uri("/Assets/Trophy-icon-Bronze.png", UriKind.RelativeOrAbsolute));
                default:
                    return new BitmapImage(new Uri("/Assets/Trophy-icon-Hidden.png", UriKind.RelativeOrAbsolute));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}