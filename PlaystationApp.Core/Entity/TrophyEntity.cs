using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class TrophyEntity
    {
        public class DefinedTrophies
        {
            public int Bronze { get; set; }
            public int Silver { get; set; }
            public int Gold { get; set; }
            public int Platinum { get; set; }
        }

        public class EarnedTrophies
        {
            public int Bronze { get; set; }
            public int Silver { get; set; }
            public int Gold { get; set; }
            public int Platinum { get; set; }
        }

        public class FromUser
        {
            public string OnlineId { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }
            public bool HiddenFlag { get; set; }
            public string LastUpdateDate { get; set; }
        }

        public class ComparedUser
        {
            public string OnlineId { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }
            public string LastUpdateDate { get; set; }
        }

        public class TrophyTitle
        {
            public string NpCommunicationId { get; set; }
            public string TrophyTitleName { get; set; }
            public string TrophyTitleDetail { get; set; }
            public string TrophyTitleIconUrl { get; set; }
            public string TrophyTitlePlatfrom { get; set; }
            public bool HasTrophyGroups { get; set; }
            public DefinedTrophies DefinedTrophies { get; set; }
            public FromUser FromUser { get; set; }
            public ComparedUser ComparedUser { get; set; }
        }
            public int TotalResults { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
            public List<TrophyTitle> TrophyTitles { get; set; }

            public static TrophyEntity Parse(JObject jobject)
            {
            string json = jobject["trophyTitles"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var trophyEntity = new TrophyEntity()
            {
                Offset = (int)jobject["offset"],
                Limit = (int)jobject["limit"],
                TotalResults = (int)jobject["totalResults"]
            };
            List<TrophyTitle> trophyTitles = (from JObject o in a
                select new TrophyTitle
                {
                    NpCommunicationId = (String)o["npCommunicationId"] ?? string.Empty,
                    TrophyTitleName = (String)o["trophyTitleName"] ?? string.Empty,
                    TrophyTitleDetail = (String)o["trophyTitleDetail"] ?? string.Empty,
                    TrophyTitleIconUrl = (String)o["trophyTitleIconUrl"] ?? string.Empty,
                    TrophyTitlePlatfrom = (String)o["trophyTitlePlatfrom"] ?? string.Empty,
                    FromUser = (JObject)o["fromUser"] != null ? ParseFromUser((JObject)o["fromUser"]) : null,
                    ComparedUser = (JObject)o["comparedUser"] != null ? ParseComparedUser((JObject)o["comparedUser"]) : null,
                    DefinedTrophies = (JObject)o["definedTrophies"] != null ? ParseDefinedTrophies((JObject)o["definedTrophies"]) : null
                }).ToList();
                trophyEntity.TrophyTitles = trophyTitles;
            return trophyEntity;
            }

        private static DefinedTrophies ParseDefinedTrophies(JObject o)
        {
            var definedTrophies = new DefinedTrophies()
            {
                Bronze = (int)o["bronze"],
                Silver = (int)o["silver"],
                Gold = (int)o["gold"],
                Platinum = (int)o["platinum"]
            };
            return definedTrophies;
        }

        private static ComparedUser ParseComparedUser(JObject o)
        {
            var comparedUser = new ComparedUser()
            {
                OnlineId = (String)o["onlineId"],
                Progress = (int)o["progress"],
                LastUpdateDate = (String)o["lastUpdateDate"],
                EarnedTrophies = (JObject)o["earnedTrophies"] != null ? ParseEarnedTrophies((JObject)o["earnedTrophies"]) : null
            };
            return comparedUser;
        }

        private static FromUser ParseFromUser(JObject o)
        {
            var fromUser = new FromUser()
            {
                OnlineId = (String)o["onlineId"],
                HiddenFlag = (bool)o["hiddenFlag"],
                Progress = (int)o["progress"],
                LastUpdateDate = (String)o["lastUpdateDate"],
                EarnedTrophies = (JObject)o["earnedTrophies"] != null ? ParseEarnedTrophies((JObject)o["earnedTrophies"]) : null
            };
            return fromUser;
        }

        private static EarnedTrophies ParseEarnedTrophies(JObject o)
        {
            var earnedTrophies = new EarnedTrophies()
            {
                Bronze = (int)o["bronze"],
                Silver = (int)o["silver"],
                Gold = (int)o["gold"],
                Platinum = (int)o["platinum"]
            };
            return earnedTrophies;
        }
    }
}
