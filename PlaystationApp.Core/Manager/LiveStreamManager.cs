using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Tools;

namespace PlaystationApp.Core.Manager
{
    public class LiveStreamManager
    {

        public async Task<TwitchEntity> GetTwitchFeed(int offset, int limit, string platform, 
            string titlePreset, string query, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                var url = UrlConstants.TwitchBaseUrl;
                // Sony's app hardcodes this value to 0. 
                // This app could, in theory, allow for more polling of data, so these options are left open to new values and limits.
                url += string.Format("offset={0}&", offset);
                url += string.Format("limit={0}&", limit);
                url += string.Format("sce_platform={0}&", platform);
                // "sce_title_preset" represents the "Interactive" filter.
                url += string.Format("sce_title_preset={0}&", titlePreset);
                url += string.Format("query={0}&", query);
                var theAuthClient = new HttpClient();

                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());

                // Twitch requires a "Platform" header to be added in order for the request to go through.
                // In this case, we can use Sony's key to let us in.
                request.Headers.Add("platform", "54bd6377db3b48cba9ecc44bff5a410b");
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }
                // Instead of parsing the object manually, let JSON.net do it for us.
                var twitchEntity = JsonConvert.DeserializeObject<TwitchEntity>(responseContent);
                return twitchEntity;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UstreamEntity> GetUstreamFeed(int pageNumber, int pageSize, string detailLevel,
            Dictionary<string, string> filterList, string sortBy, string query, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                var url = UrlConstants.UstreamBaseUrl;
                url += string.Format("p={0}&", pageNumber);
                url += string.Format("pagesize={0}&", pageSize);
                
                url += string.Format("detail_level={0}&", detailLevel);
                foreach (var item in filterList)
                {
                    if (item.Key.Equals(UstreamUrlConstants.Interactive))
                    {
                        url += string.Format(UstreamUrlConstants.FilterBase, UstreamUrlConstants.PlatformPs4) +
                                         "[interactive]=" + item.Value + "&";
                    }
                    else
                    {
                        url += string.Concat(string.Format(UstreamUrlConstants.FilterBase, item.Key), "=", item.Value + "&");
                    }
                }
                url += string.Format("sort={0}", sortBy);
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
                // Instead of parsing the object manually, let JSON.net do it for us.
                var ustreamEntity = JsonConvert.DeserializeObject<UstreamEntity>(responseContent);
                return ustreamEntity;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    public class LiveStreamEntity
    {
        public UstreamEntity Ustream { get; set; }

        public TwitchEntity Twitch { get; set; }
    }
}
