using System;
using System.Globalization;
using System.Windows.Data;
using PlaystationApp.Resources;

namespace PlaystationApp.Tools
{
    public class TrophyGradeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var trophyType = (string) value;
            switch (trophyType)
            {
                case "platinum":
                    return AppResources.TrophyPlatinum;
                case "gold":
                    return AppResources.TrophyGold;
                case "silver":
                    return AppResources.TrophySilver;
                case "bronze":
                    return AppResources.TrophyBronze;
                default:
                    return AppResources.TrophyHidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}