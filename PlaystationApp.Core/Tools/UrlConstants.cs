using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Tools
{
    public class UrlConstants
    {
        public const string BaseUrl = "https://jp-tpy.np.community.playstation.net";

        public const string NotificationList = BaseUrl + "/notificationList/v1/users/{0}/notifications?fields=%40default%2Cmessage%2CactionUrl&npLanguage=ja";
    }
}
