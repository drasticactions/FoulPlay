using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class UserAccountEntity
    {
        private User _entity;
        private AccountData _data;
        private Boolean _isCalled;
        private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

        public String GetAccessToken()
        {
            if (GetUnixTime(DateTime.Now) - this._data.StartTime >= this._data.RefreshTime)
            {
                if (!this._isCalled)
                {
                    this._isCalled = true;
                    return "refresh";
                }
            }
            return this._data.AccessToken;
        }

        public UserAccountEntity()
        {
            string accessToken = "";
            string refreshToken = "";
            _appSettings.TryGetValue("accessToken", out accessToken);
            _appSettings.TryGetValue("refreshToken", out refreshToken);
            this._data = new AccountData(accessToken, refreshToken, 3600);
            this._entity = null;
            this._isCalled = false;
        }

        public void SetUserEntity(User entity)
        {
            this._entity = entity;
        }

        public User GetUserEntity()
        {
            return this._entity;
        }

        public void SetAccessToken(String token, String refresh)
        {
            this._data.AccessToken = token;
            this._data.RefreshToken = refresh;
            if (this._data.RefreshToken != null)
            {
                this._isCalled = false;
            }
        }

        public void SetRefreshTime(long time)
        {
            this._data.RefreshTime = time;
            this._data.StartTime = GetUnixTime(DateTime.Now);
        }

        private class AccountData
        {
            public AccountData(String token, String refresh, int time)
            {
                this.AccessToken = token;
                this.RefreshToken = refresh;
                this.RefreshTime = time;
                this.StartTime = UserAccountEntity.GetUnixTime(DateTime.Now);
            }

            public String AccessToken;
            public String RefreshToken;
            public long RefreshTime;
            public long StartTime;
        }

        public String GetRefreshToken()
        {
            return this._data.RefreshToken;
        }

        public static long GetUnixTime(DateTime time)
        {
            time = time.ToUniversalTime();
            TimeSpan timeSpam = time - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            return (long)timeSpam.TotalSeconds;
        }

        public override string ToString()
        {
            return this.GetAccessToken() + ":" + this.GetRefreshToken();
        }

        public class User
        {
            public string AccountId { get; set; }

            public string MAccountId { get; set; }

            public string Region { get; set; }

            public string Language { get; set; }

            public string OnlineId { get; set; }

            public string Age { get; set; }

            public string DateOfBirth { get; set; }

            public string CommunityDomain { get; set; }

            public bool SubAccount { get; set; }

            public bool Ps4Available { get; set; }
        }

        public static User ParseUser(string json)
        {
            var o = JObject.Parse(json);
            return new User()
            {
                AccountId = (String)o["accountId"] ?? string.Empty,
                MAccountId = (String)o["mAccountId"] ?? string.Empty,
                Region = (String)o["region"] ?? string.Empty,
                Language = (String)o["language"] ?? string.Empty,
                OnlineId = (String)o["onlineId"] ?? string.Empty,
                Age = (String)o["age"] ?? string.Empty,
                DateOfBirth = (String)o["dateOfBirth"] ?? string.Empty,
                CommunityDomain = (String)o["communityDomain"] ?? string.Empty,
                Ps4Available = o["ps4Available"] != null && (bool)o["ps4Available"],
                SubAccount = o["subaccount"] != null && (bool)o["subaccount"]
            };
        }
    }
}
