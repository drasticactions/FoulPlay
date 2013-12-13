using System;
using System.Globalization;
using System.Windows.Data;
using PlaystationApp.Resources;

namespace PlaystationApp.Tools
{
    public class TrophyRareTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var trophyValue = (int) value;
            switch (trophyValue)
            {
                case 0:
                    return AppResources.TrophyUltraRare;
                case 1:
                    return AppResources.TrophyVeryRare;
                case 2:
                    return AppResources.TrophyRare;
                case 3:
                    return AppResources.TrophyCommon;
                case 4:
                    return AppResources.TrophyCommon;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}