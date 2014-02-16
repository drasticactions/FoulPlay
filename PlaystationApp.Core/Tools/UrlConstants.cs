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

        public const string UstreamBaseUrl = "https://ps4api.ustream.tv/media.json?";

        public const string TwitchBaseUrl = "https://api.twitch.tv/api/orbis/streams?";

        public const string NotificationList = BaseUrl + "/notificationList/v1/users/{0}/notifications?fields=%40default%2Cmessage%2CactionUrl&npLanguage=ja";
    }

    public class UstreamUrlConstants
    {
        public const string FilterBase = "filter[{0}]";

        public const string Platform = "platform";

        public const string Type = "type";

        public const string PlatformPs4 = "PS4";

        public const string Interactive = "interactive";

        public const string Sort = "sort";
    }
}
