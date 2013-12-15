using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Core.Manager
{
    public class FriendManager
    {
        public async Task<FriendsEntity> GetFriendsList(string username, int? offset, bool blockedPlayer, bool playedRecently, bool personalDetailSharing, bool friendStatus, bool requesting, bool requested, bool onlineFilter,  UserAccountEntity userAccountEntity)
        {
            var authenticationManager = new AuthenticationManager();
            var user = userAccountEntity.GetUserEntity();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList?fields=@default,relation,onlineId%2CavatarUrl%2Cplus%2CpersonalDetail%2CtrophySummary&sort=onlineId&avatarSize=m&offset={2}&limit=32", user.Region, username, offset);
            if (onlineFilter) url += "&filter=online";
            if (friendStatus && !requesting && !requested) url += "&friendStatus=friend&presenceType=primary";
            if (friendStatus && requesting && !requested) url += "&friendStatus=requesting";
            if (friendStatus && !requesting && requested) url += "&friendStatus=requested";
            if (personalDetailSharing && requested) url += "&friendStatus=friend&personalDetailSharing=requested&presenceType=primary";
            if (personalDetailSharing && requesting) url += "&friendStatus=friend&personalDetailSharing=requesting&presenceType=primary";
            if (playedRecently)
                url =
                    string.Format(
                        "https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/recentlyplayed?", username);
            if (blockedPlayer) url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/blockList?fields=%40default%2C%40profile&offset={2}", user.Region, username, offset);
            // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
            url += "&r=" + Guid.NewGuid();
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            request.Headers.CacheControl = new CacheControlHeaderValue{NoCache = true, MustRevalidate = true};
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent)) return null;
            responseContent = "[" + responseContent + "]";
            JArray a = JArray.Parse(responseContent);
            var b = (JObject)a[0];
            var friend = FriendsEntity.Parse(b);
            return friend;
        }

        public async Task<bool> DenyAddFriend(bool isRemove, string username, UserAccountEntity userAccountEntity)
        {
            var user = userAccountEntity.GetUserEntity();
            var authenticationManager = new AuthenticationManager();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var theAuthClient = new HttpClient();
            var url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}", user.Region, user.OnlineId, username);
            var httpMethod = isRemove ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            var response = await theAuthClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetRequestMessage(string username, UserAccountEntity userAccountEntity)
        {
            var user = userAccountEntity.GetUserEntity();
            var authenticationManager = new AuthenticationManager();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var theAuthClient = new HttpClient();
            var url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}/requestMessage", user.Region, user.OnlineId, username);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            var response = await theAuthClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var o = JObject.Parse(responseContent);
            return (string)o["requestMessage"];
        }

        public async Task<bool> SendFriendRequest(string username, string requestMessage,
            UserAccountEntity userAccountEntity)
        {
            var user = userAccountEntity.GetUserEntity();
            var authenticationManager = new AuthenticationManager();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var theAuthClient = new HttpClient();
            var param = new Dictionary<String, String>();
            var url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}?requestMessage={3}", user.Region, user.OnlineId, username, requestMessage);
            if(!string.IsNullOrEmpty(requestMessage)) param.Add("requestMessage", requestMessage);
            var jsonObject = JsonConvert.SerializeObject(param);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) {Content = stringContent};
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            string requestContent = await request.Content.ReadAsStringAsync();
            var response = await theAuthClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendNameRequest(string username, UserAccountEntity userAccountEntity)
        {
            var user = userAccountEntity.GetUserEntity();
            var authenticationManager = new AuthenticationManager();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var theAuthClient = new HttpClient();
            var param = new Dictionary<String, String>();
            var url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}/personalDetailSharing", user.Region, user.OnlineId, username);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            //string requestContent = await request.Content.ReadAsStringAsync();
            var response = await theAuthClient.SendAsync(request);
            return response.IsSuccessStatusCode;   
        }
    }
}
