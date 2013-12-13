using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class UserEntity
    {
        public string OnlineId { get; set; }
        public string Region { get; set; }
        public string NpId { get; set; }
        public string AvatarUrl { get; set; }
        public string PanelBgc { get; set; }
        public string PanelUrl { get; set; }
        public string AboutMe { get; set; }
        public List<string> LanguagesUsed { get; set; }
        public bool Plus { get; set; }
        public TrophySummary trophySummary { get; set; }
        public string Relation { get; set; }
        public Presence presence { get; set; }
        public string PersonalDetailSharing { get; set; }
        public bool RequestMessageFlag { get; set; }

        public PersonalDetail personalDetail { get; set; }

        public static UserEntity Parse(String json)
        {
            if (json == null) throw new ArgumentNullException("json");
            JObject o = JObject.Parse(json);
            var user = new UserEntity
            {
                OnlineId = (String)o["onlineId"] ?? string.Empty,
                Region = (String)o["region"] ?? string.Empty,
                NpId = (String)o["npId"] ?? string.Empty,
                AvatarUrl = (String)o["avatarUrl"] ?? string.Empty,
                LanguagesUsed = o["languagesUsed"] != null ? JsonConvert.DeserializeObject<List<string>>(o["languagesUsed"].ToString()) : null,
                PanelBgc = (String)o["panelBgc"] ?? string.Empty,
                PanelUrl = (String)o["panelUrl"] ?? string.Empty,
                AboutMe = (String)o["aboutMe"] ?? string.Empty,
                personalDetail = (JObject)o["personalDetail"] != null ? ParsePersonalDetail((JObject)o["personalDetail"]) : null,
                Plus = o["plus"]!= null && (Boolean)o["plus"],
                presence = o["presence"] != null ? ParsePresence((JObject)o["presence"]) : null,
                trophySummary = (JObject)o["trophySummary"] != null ? ParseTrophySummary((JObject)o["trophySummary"]) : null,
                Relation = (String)o["relation"] ?? string.Empty,
                RequestMessageFlag = o["requestMessageFlag"] != null && (bool)o["requestMessageFlag"]
            };
            return user;
        }

        private static Presence ParsePresence(JObject o)
        {
            var presense = new Presence
            {
                PrimaryInfo = ParsePrimaryInfo((JObject)o["primaryInfo"])
            };
            return presense;
        }

        private static PersonalDetail ParsePersonalDetail(JObject o)
        {
            var personalDetail = new PersonalDetail()
            {
                FirstName = (String)o["firstName"],
                LastName = (String)o["lastName"],
                FullName = string.Format("{0} {1}", (String)o["firstName"], (String)o["lastName"]),
                ProfilePictureUrl = (String)o["profilePictureUrl"]
            };
            return personalDetail;
        }

        public class EarnedTrophies
        {
            public int Platinum { get; set; }
            public int Gold { get; set; }
            public int Silver { get; set; }
            public int Bronze { get; set; }
        }

        public class TrophySummary
        {
            public int Level { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }

            public int TotalTrophies { get; set; }
        }


        public class PrimaryInfo
        {
            public string Platform { get; set; }
            public string OnlineStatus { get; set; }
            public GameTitleInfo GameTitleInfo { get; set; }
            public string GameStatus { get; set; }
            public string LastOnlineDate { get; set; }
        }

        public class GameTitleInfo
        {
            public string NpTitleId { get; set; }
            public string TitleName { get; set; }
        }

        public class Presence
        {
            public PrimaryInfo PrimaryInfo { get; set; }
        }

        public class PersonalDetail
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string FullName { get; set; }
            public string ProfilePictureUrl { get; set; }
        }

        private static TrophySummary ParseTrophySummary(JObject o)
        {
            var trophySummary = new TrophySummary
            {
                Level = (int)o["level"],
                Progress = (int)o["progress"],
                EarnedTrophies = ParseEarnedTrophies((JObject)o["earnedTrophies"])
            };
            trophySummary.TotalTrophies = trophySummary.EarnedTrophies.Bronze +
                                          trophySummary.EarnedTrophies.Silver +
                                          trophySummary.EarnedTrophies.Gold +
                                          trophySummary.EarnedTrophies.Platinum;
            return trophySummary;
        }

        private static EarnedTrophies ParseEarnedTrophies(JObject o)
        {
            var earnedTrophies = new EarnedTrophies
            {
                Bronze = (int)o["bronze"],
                Silver = (int)o["silver"],
                Gold = (int)o["gold"],
                Platinum = (int)o["platinum"]
            };
            return earnedTrophies;
        }

        private static PrimaryInfo ParsePrimaryInfo(JObject o)
        {
            var primaryInfo = new PrimaryInfo
            {
                Platform = (String)o["platform"],
                OnlineStatus = (String)o["onlineStatus"],
                GameStatus = (String)o["gameStatus"] ?? string.Empty,
                GameTitleInfo = o["gameTitleInfo"] != null ? ParseGameTitleInfo((JObject)o["gameTitleInfo"]) : null,
                LastOnlineDate = (String)o["lastOnlineDate"] ?? string.Empty
            };
            return primaryInfo;
        }

        private static GameTitleInfo ParseGameTitleInfo(JObject o)
        {
            var gameTitleInfo = new GameTitleInfo
            {
                NpTitleId = (String)o["npTitleId"],
                TitleName = (String)o["titleName"]
            };
            return gameTitleInfo;
        }

    }
}
