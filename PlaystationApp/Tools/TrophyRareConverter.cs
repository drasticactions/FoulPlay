using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PlaystationApp.Tools
{
    public class TrophyRareConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var trophyValue = (int) value;
            switch (trophyValue)
            {
                case 0:
                    return
                        new BitmapImage(new Uri("/Images/phone_trophy_rareness_ultraRare.png",
                            UriKind.RelativeOrAbsolute));
                case 1:
                    return new BitmapImage(new Uri("/Images/phone_trophy_rareness_rare.png", UriKind.RelativeOrAbsolute));
                case 2:
                    return
                        new BitmapImage(new Uri("/Images/phone_trophy_rareness_uncommon.png", UriKind.RelativeOrAbsolute));
                case 3:
                    return
                        new BitmapImage(new Uri("/Images/phone_trophy_rareness_common.png", UriKind.RelativeOrAbsolute));
                case 4:
                    return
                        new BitmapImage(new Uri("/Images/phone_trophy_rareness_common.png", UriKind.RelativeOrAbsolute));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}