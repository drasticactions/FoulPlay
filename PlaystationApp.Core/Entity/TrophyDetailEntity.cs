using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class TrophyDetailEntity
    {
        public class FromUser
        {
            public string OnlineId { get; set; }
            public bool Earned { get; set; }
            public string EarnedDate { get; set; }
        }

        public class ComparedUser
        {
            public string OnlineId { get; set; }
            public bool Earned { get; set; }
            public string EarnedDate { get; set; }
        }

        public class Trophy
        {
            public int TrophyId { get; set; }
            public bool TrophyHidden { get; set; }
            public string TrophyType { get; set; }
            public string TrophyName { get; set; }
            public string TrophyDetail { get; set; }
            public string TrophyIconUrl { get; set; }
            public int TrophyRare { get; set; }
            public string TrophyEarnedRate { get; set; }
            public FromUser FromUser { get; set; }
            public ComparedUser ComparedUser { get; set; }
        }

        public static TrophyDetailEntity Parse(JObject jobject)
        {
            string json = jobject["trophies"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            List<Trophy> trophies = (from JObject o in a
                select new Trophy()
                {
                    TrophyDetail = (String)o["trophyDetail"] ?? string.Empty,
                    TrophyEarnedRate = (String)o["trophyEarnedRate"] ?? string.Empty,
                    TrophyHidden = (Boolean)o["trophyHidden"],
                    TrophyIconUrl = (String)o["trophyIconUrl"] ?? "/Assets/No-Trophy-Icon.png",
                    TrophyId = (int)o["trophyId"],
                    TrophyName = (String)o["trophyName"] ?? string.Empty,
                    TrophyRare = (int)o["trophyRare"],
                    FromUser = (JObject)o["fromUser"] != null ? ParseFromUser((JObject)o["fromUser"]) : null,
                    ComparedUser = (JObject)o["comparedUser"] != null ? ParseComparedUser((JObject)o["comparedUser"]) : null,
                    TrophyType = (String)o["trophyType"] ?? string.Empty,
                }).ToList();
            var trophyDetailEntity = new TrophyDetailEntity()
            {
                Trophies = trophies
            };
            return trophyDetailEntity;
        }

        private static ComparedUser ParseComparedUser(JObject o)
        {
            var comparedUser = new ComparedUser()
            {
                OnlineId = (String) o["onlineId"],
                Earned = (Boolean) o["earned"],
                EarnedDate =
                    o["earnedDate"] != null ? (string) o["earnedDate"] : string.Empty
            };
            return comparedUser;
        }

        private static FromUser ParseFromUser(JObject o)
        {
            var fromUser = new FromUser()
            {
                OnlineId = (String)o["onlineId"],
                Earned = (Boolean)o["earned"],
                EarnedDate = o["earnedDate"] != null ? (string)o["earnedDate"] : string.Empty
            };
            return fromUser;
        }



        public List<Trophy> Trophies { get; set; }

    }
}
