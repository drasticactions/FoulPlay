using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Entity;

namespace PlaystationApp.Core.Manager
{

    public class AuthenticationManager
    {
        private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

        private const string ConsumerKey = "4db3729d-4591-457a-807a-1cf01e60c3ac";

        private const string ConsumerSecret = "criemouwIuVoa4iU";

        private const string OauthToken = "https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/token";

        public async Task<UserAccountEntity.User> GetUserEntity(UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = "https://vl.api.np.km.playstation.net/vl/api/v1/mobile/users/me/info";
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                UserAccountEntity.User user = UserAccountEntity.ParseUser(responseContent);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public async Task<bool> RequestAccessToken(String code)
        {
            try
            {
                var dic = new Dictionary<String, String>();
                dic["grant_type"] = "authorization_code";
                dic["client_id"] = ConsumerKey;
                dic["client_secret"] = ConsumerSecret;
                dic["redirect_uri"] = "com.playstation.PlayStationApp://redirect";
                dic["state"] = "x";
                dic["scope"] = "psn:sceapp";
                dic["code"] = code;
                var theAuthClient = new HttpClient();
                var header = new FormUrlEncodedContent(dic);
                var response = await theAuthClient.PostAsync(OauthToken, header);
                string responseContent = await response.Content.ReadAsStringAsync();
                UserAuthenticationEntity authEntity = UserAuthenticationEntity.Parse(responseContent);
                if (!_appSettings.Any())
                {
                    _appSettings.Add("accessToken", authEntity.AccessToken);
                }
                else
                {
                    _appSettings["accessToken"] = authEntity.AccessToken;
                }

                if (!_appSettings.Any())
                {
                    _appSettings.Add("refreshToken", authEntity.RefreshToken);
                }
                else
                {
                    _appSettings["refreshToken"] = authEntity.RefreshToken;
                }

                _appSettings.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async Task<bool> RefreshAccessToken(UserAccountEntity account)
        {
            try
            {
                //var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                var dic = new Dictionary<String, String>();
                dic["grant_type"] = "refresh_token";
                dic["client_id"] = ConsumerKey;
                dic["client_secret"] = ConsumerSecret;
                dic["refresh_token"] = account.GetRefreshToken();
                dic["scope"] = "psn:sceapp";

                account.SetAccessToken("updating", null);
                account.SetRefreshTime(1000);
                var theAuthClient = new HttpClient();
                HttpContent header = new FormUrlEncodedContent(dic);
                HttpResponseMessage response = await theAuthClient.PostAsync(OauthToken, header);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject o = JObject.Parse(responseContent);
                    account.SetAccessToken((String)o["access_token"], (String)o["refresh_token"]);
                    account.SetRefreshTime(long.Parse((String)o["expires_in"]));

                    UserAuthenticationEntity authEntity = UserAuthenticationEntity.Parse(responseContent);
                    _appSettings["refreshToken"] = authEntity.RefreshToken;
                    _appSettings["accessToken"] = authEntity.AccessToken;
                    _appSettings.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
