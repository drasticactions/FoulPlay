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
    public class TrophyManager
    {
        public async Task<TrophyEntity> GetTrophyList(string user, int offset, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var userAccount = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-tpy.np.community.playstation.net/trophy/v1/trophyTitles?fields=%40default&npLanguage={1}&iconSize=s&platform=PS3%2CPSVITA%2CPS4&offset={2}&limit=64&comparedUser={3}&fromUser={4}", userAccount.Region, userAccount.Language, offset, user, userAccountEntity.GetUserEntity().OnlineId);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                var trophy = JsonConvert.DeserializeObject<TrophyEntity>(responseContent);
                return trophy;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
