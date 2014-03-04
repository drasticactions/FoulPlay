using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Entity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Manager
{
    public class RecentActivityManager
    {
        public async Task<RecentActivityEntity> GetActivityFeed(string userName, int? pageNumber, bool storePromo, bool isNews, UserAccountEntity userAccountEntity)
        {
            try
            {
            var authenticationManager = new AuthenticationManager();
            var feedNews = isNews ? "news" : "feed";
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = string.Format("https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/{1}/{2}?filters=PLAYED_GAME&filters=TROPHY&filters=BROADCASTING&filters=PROFILE_PIC&filters=FRIENDED", userName, feedNews, pageNumber);
            // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
            url += "&r=" + Guid.NewGuid();
            var theAuthClient = new HttpClient();
            string language = userAccountEntity.GetUserEntity().Language;
            theAuthClient.DefaultRequestHeaders.Add("Accept-Language", language);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent)) return null;
                var recentActivityEntity = JsonConvert.DeserializeObject<RecentActivityEntity>(responseContent);
                return recentActivityEntity;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public async Task<bool> LikeDislikeFeedItem(bool isLiked, string feedId, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var liked = isLiked ? "like" : "dislike";
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/set/{1}/story/{2}", userAccountEntity.GetUserEntity().OnlineId, liked, feedId);
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                var response = await theAuthClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
           
        }
    }
}
