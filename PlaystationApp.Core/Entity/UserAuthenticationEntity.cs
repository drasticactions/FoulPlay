using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class UserAuthenticationEntity
    {

        public string AccessToken { get; private set; }

        public string RefreshToken { get; private set; }

        public long ExpiresIn { get; private set; }

        public string TokenType { get; private set; }

        public string Scope { get; private set; }

        public static UserAuthenticationEntity Parse(string json)
        {
            var o = JObject.Parse(json);
            var authEntity = new UserAuthenticationEntity
            {
                AccessToken = (String) o["access_token"],
                RefreshToken = (String) o["refresh_token"],
                ExpiresIn = (long) o["expires_in"],
                TokenType = (String) o["token_type"],
                Scope = (String) o["scope"]
            };
            return authEntity;
        }
    }
}
