using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class FriendsEntity
    {
        public List<Friend> FriendList { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }

        public static FriendsEntity Parse(JObject jobject)
        {
            string json = jobject["friendList"] != null ? jobject["friendList"].ToString() : null;
            if (string.IsNullOrEmpty(json)) return null;
            var a = (JArray) JsonConvert.DeserializeObject(json);
            var friendsEntity = new FriendsEntity
            {
                Size = jobject["size"] != null ? (int) jobject["size"] : 0,
                Start = jobject["start"] != null ? (int) jobject["start"] : 0,
                TotalResults = jobject["totalResults"] != null ? (int) jobject["totalResults"] : 0
            };
            List<Friend> friendList = (from JObject o in a
                select new Friend
                {
                    AvatarUrl =  (String) o["avatarUrl"] ?? string.Empty,
                    OnlineId = (String)o["onlineId"] ?? string.Empty,
                    Plus = o["plus"] != null && (Boolean) o["plus"],
                    TrophySummary = o["trophySummary"] != null ? ParseTrophySummary((JObject) o["trophySummary"]) : null,
                    Presence = o["presence"] != null ? ParsePresence((JObject)o["presence"]) : null,
                    PersonalDetail = (JObject)o["personalDetail"] != null ? ParsePersonalDetail((JObject)o["personalDetail"]) : null
                }).ToList();
            friendsEntity.FriendList = friendList;
            return friendsEntity;
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
                FirstName = (String)o["firstName"] ?? string.Empty,
                LastName = (String)o["lastName"] ?? string.Empty,
                FullName = string.Format("{0} {1}", (String)o["firstName"] ?? string.Empty, (String)o["lastName"] ?? string.Empty),
                ProfilePictureUrl = (String)o["profilePictureUrl"] ?? string.Empty
            };
            return personalDetail;
        }

        private static PrimaryInfo ParsePrimaryInfo(JObject o)
        {
            var primaryInfo = new PrimaryInfo
            {
                Platform = (String) o["platform"] ?? string.Empty,
                OnlineStatus = (String)o["onlineStatus"] ?? string.Empty,
                GameStatus = (String)o["gameStatus"] ?? string.Empty,
                GameTitleInfo = o["gameTitleInfo"] != null ? ParseGameTitleInfo((JObject) o["gameTitleInfo"]) : null,
                LastOnlineDate = o["lateOnlineDate"] != null ? DateTime.Parse((string)o["lateOnlineDate"]).ToLocalTime() : new DateTime()
            };
            primaryInfo.IsOnline = primaryInfo.OnlineStatus.Equals("Online");
            return primaryInfo;
        }

        private static GameTitleInfo ParseGameTitleInfo(JObject o)
        {
            var gameTitleInfo = new GameTitleInfo
            {
                NpTitleId = (String) o["npTitleId"],
                TitleName = (String) o["titleName"]
            };
            return gameTitleInfo;
        }

        private static TrophySummary ParseTrophySummary(JObject o)
        {
            var trophySummary = new TrophySummary
            {
                Level = (int) o["level"],
                Progress = (int) o["progress"],
                EarnedTrophies = ParseEarnedTrophies((JObject) o["earnedTrophies"])
            };
            return trophySummary;
        }

        private static EarnedTrophies ParseEarnedTrophies(JObject o)
        {
            var earnedTrophies = new EarnedTrophies
            {
                Bronze = (int) o["bronze"],
                Silver = (int) o["silver"],
                Gold = (int) o["gold"],
                Platinum = (int) o["platinum"]
            };
            return earnedTrophies;
        }

        private static long GetUnixTime(DateTime time)
        {
            time = time.ToUniversalTime();
            TimeSpan timeSpam = time - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            return (long) timeSpam.TotalSeconds;
        }

        public class EarnedTrophies
        {
            public int Platinum { get; set; }
            public int Gold { get; set; }
            public int Silver { get; set; }
            public int Bronze { get; set; }
        }

        public class Friend
        {
            public string OnlineId { get; set; }
            public string AvatarUrl { get; set; }

            public Boolean Plus { get; set; }
            public TrophySummary TrophySummary { get; set; }
            public Presence Presence { get; set; }
            public PersonalDetail PersonalDetail { get; set; }
        }

        public class GameTitleInfo
        {
            public string NpTitleId { get; set; }
            public string TitleName { get; set; }
        }

        public class PersonalDetail
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName { get; set; }
            public string ProfilePictureUrl { get; set; }
        }

        public class Presence
        {
            public PrimaryInfo PrimaryInfo { get; set; }
        }

        public class PrimaryInfo
        {
            public string Platform { get; set; }
            public string OnlineStatus { get; set; }
            public bool IsOnline { get; set; }
            public GameTitleInfo GameTitleInfo { get; set; }
            public string GameStatus { get; set; }
            public DateTime LastOnlineDate { get; set; }
        }

        public class TrophySummary
        {
            public int Level { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }
        }
    }
}