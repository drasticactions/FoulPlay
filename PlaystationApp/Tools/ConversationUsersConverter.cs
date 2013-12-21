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
    public class ConversationUsersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var message = (MessageGroupEntity.MessageGroup)value;
            var messageGroupDetail = message.MessageGroupDetail;
            var user = App.UserAccountEntity.GetUserEntity();
            var stringEnumerable = messageGroupDetail.Members.Where(member => !member.OnlineId.Equals(user.OnlineId)).Select(member => member.OnlineId).ToList();
            return string.Join<string>(",", stringEnumerable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
