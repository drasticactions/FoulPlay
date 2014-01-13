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
    public class SessionInviteManager
    {
        public async Task<SessionInviteEntity> GetSessionInvites(int offset, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-ivt.np.community.playstation.net/sessionInvitation/v1/users/{1}/invitations?fields=@default,sessionId,receivedDate,expired,updateDate,fromUser,subject,npTitleDetail,availablePlatforms&npLanguage={2}&offset={3}", user.Region, user.OnlineId, user.Language, offset);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent)) return null;
                responseContent = "[" + responseContent + "]";
                var a = JArray.Parse(responseContent);
                var b = (JObject)a[0];
                var sessionInvite = SessionInviteEntity.Parse(b);
                return sessionInvite;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public async Task<SessionInviteDetailEntity> GetInviteInformation(string inviteId, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-ivt.np.community.playstation.net/sessionInvitation/v1/users/{1}/invitations/{2}?fields=@default,npTitleDetail,session,members&npLanguage={3}", user.Region, user.OnlineId, inviteId, user.Language);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                var response = await theAuthClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent)) return null;
                responseContent = "[" + responseContent + "]";
                var a = JArray.Parse(responseContent);
                var b = (JObject)a[0];
                var sessionInvite = SessionInviteDetailEntity.Parse(b);
                return sessionInvite;  
            }
            catch (Exception)
            {
                return null;
            } 
        }
    }
}
