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
    public class TrophyDetailManager
    {
        public async Task<TrophyDetailEntity> GetTrophyDetailList(string gameId, string comparedUser, bool includeHidden, UserAccountEntity userAccountEntity)
        {
            var authenticationManager = new AuthenticationManager();
            var user = userAccountEntity.GetUserEntity();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = string.Format("https://{0}-tpy.np.community.playstation.net/trophy/v1/trophyTitles/{1}/trophyGroups/all/trophies?fields=@default,trophyRare,trophyEarnedRate&npLanguage={2}&iconSize=m&comparedUser={3}&fromUser={4}", user.Region, gameId, user.Language, comparedUser, user.OnlineId);
            var theAuthClient = new HttpClient();
            // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
            url += "&r=" + Guid.NewGuid();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            responseContent = "[" + responseContent + "]";
            JArray a = JArray.Parse(responseContent);
            var b = (JObject)a[0];
            var trophy = TrophyDetailEntity.Parse(b);
            if (!includeHidden)
            {
                trophy.Trophies = trophy.Trophies.Where(trophyDetail => trophyDetail.TrophyHidden != true).ToList();
            }
            return trophy;
        }
    }
}
