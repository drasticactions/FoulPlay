using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Core.Manager
{
    public class RecentActivityManager
    {
        public async Task<RecentActivityEntity> GetActivityFeed(string userName, int? pageNumber, bool storePromo, bool isNews, UserAccountEntity userAccountEntity)
        {
            var authenticationManager = new AuthenticationManager();
            var feedNews = isNews ? "news" : "feed";
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = string.Format("https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/{1}/{2}?filters=PLAYED_GAME&filters=TROPHY&filters=BROADCASTING&filters=PROFILE_PIC&filters=FRIENDED", userName, feedNews, pageNumber);
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            var response = await theAuthClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent)) return null;
            responseContent = "[" + responseContent + "]";
            JArray a = JArray.Parse(responseContent);
            var b = (JObject)a[0];
            if (b["message"] != null) return null;
            var recentActivityEntity = RecentActivityEntity.Parse(b["feed"].ToString());
            return recentActivityEntity;
        }
    }
}
