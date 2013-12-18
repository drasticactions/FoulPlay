using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Core.Manager
{
    public class SessionInviteManager
    {
        public async Task<bool> GetSessionInvites(UserAccountEntity userAccountEntity)
        {
            var authenticationManager = new AuthenticationManager();
            var user = userAccountEntity.GetUserEntity();
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await authenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = string.Format("https://{0}-ivt.np.community.playstation.net/sessionInvitation/v1/users/{1}/invitations?fields=@default,sessionId,receivedDate,expired,updateDate,fromUser,subject,npTitleDetail,availablePlatforms&npLanguage={2}", user.Region, user.OnlineId, user.Language);
            // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
            url += "&r=" + Guid.NewGuid();
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            var response = await theAuthClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent)) return false;
            return true;
        }
    }
}
