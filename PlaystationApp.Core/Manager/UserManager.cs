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
    public class UserManager
    {
        public async Task<UserEntity> GetUser(string userName, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/profile?fields=@default,relation,onlineId,presence,avatarUrl,plus,personalDetail,trophySummary", user.Region, userName);
                var theAuthClient = new HttpClient();
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                var friend = JsonConvert.DeserializeObject<UserEntity>(responseContent);
                return friend;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public async static Task<UserEntity> GetUserAvatar(string userName, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/profile?fields=avatarUrl", user.Region, userName);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                var friend = JsonConvert.DeserializeObject<UserEntity>(responseContent);
                return friend;
            }
            catch (Exception)
            {
                return null;
            }
           
        }
    }
}
