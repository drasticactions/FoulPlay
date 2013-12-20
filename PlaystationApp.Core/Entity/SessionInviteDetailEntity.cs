using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class SessionInviteDetailEntity
    {

        public string ReceivedDate { get; set; }
        public bool SeenFlag { get; set; }
        public bool Expired { get; set; }
        public string Message { get; set; }
        public List<string> AvailablePlatforms { get; set; }
        public FromUser fromUser { get; set; }
        public bool UsedFlag { get; set; }
        public Session session { get; set; }
        public NpTitleDetail npTitleDetail { get; set; }

        public class FromUser
        {
            public string OnlineId { get; set; }
        }

        public class Member
        {
            public string OnlineId { get; set; }
            public string Platform { get; set; }

            public string AvatarUrl { get; set; }
        }

        public class Session
        {
            public string SessionId { get; set; }
            public string NpTitleType { get; set; }
            public string SessionType { get; set; }
            public string SessionPrivacy { get; set; }
            public int SessionMaxUser { get; set; }
            public string SessionName { get; set; }
            public string SessionStatus { get; set; }
            public long SessionCreateTimestamp { get; set; }
            public string SessionCreator { get; set; }
            public List<Member> Members { get; set; }
        }

        public class NpTitleDetail
        {
            public string NpTitleId { get; set; }
            public string NpCommunicationId { get; set; }
            public string NpTitleName { get; set; }
            public string NpTitleIconUrl { get; set; }
        }

        public static SessionInviteDetailEntity Parse(JObject o)
        {
            var sessionInviteEntity = new SessionInviteDetailEntity
            {
                Message = o["message"] != null ? (string)o["message"] : string.Empty,
                SeenFlag = o["seenFlag"] != null && (bool)o["seenFlag"],
                UsedFlag = o["usedFlag"] != null && (bool)o["usedFlag"],
                AvailablePlatforms = o["availablePlatforms"] != null ? (from platform in o["availablePlatforms"] select platform.ToString()).ToList() : new List<string>(),
                ReceivedDate = o["receivedDate"] != null ? (string)o["receivedDate"] : string.Empty,
                Expired = o["expired"] != null && (bool)o["expired"],
                session = (JObject)o["session"] != null ? ParseSession((JObject)o["session"]) : null,
                fromUser = (JObject)o["fromUser"] != null ? ParseFromUser((JObject)o["fromUser"]) : null,
                npTitleDetail = (JObject)o["npTitleDetail"] != null ? ParseTitleDetail((JObject)o["npTitleDetail"]) : null
            };
            return sessionInviteEntity;
        }

        private static FromUser ParseFromUser(JObject o)
        {
            var fromUser = new FromUser()
            {
                OnlineId = (String)o["onlineId"]
            };
            return fromUser;
        }

        private static Session ParseSession(JObject o)
        {
            return new Session
            {
                NpTitleType = (string) o["npTitleType"] ?? string.Empty,
                SessionId = (string)o["sessionId"] ?? string.Empty,
                SessionType = (string)o["sessionType"] ?? string.Empty,
                SessionCreateTimestamp = o["sessionCreateTimestamp"] != null ? (long)o["sessionCreateTimestamp"] : 0,
                SessionCreator = (string)o["sessionCreator"] ?? string.Empty,
                Members = (JArray)o["members"] != null ? ParseMembers(o) : null,
                SessionMaxUser = o["sessionMaxUser"] != null ? (int)o["sessionMaxUser"] : 0,
                SessionName = (string)o["sessionName"] ?? string.Empty,
                SessionPrivacy = (string)o["sessionPrivacy"] ?? string.Empty,
                SessionStatus = (string)o["sessionStatus"] ?? string.Empty
            };
        }

        private static List<Member> ParseMembers(JObject obj)
        {
            string json = obj["members"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var members = (from JObject o in a
                select new Member
                {
                    OnlineId = (String)o["onlineId"] ?? string.Empty,
                    Platform = (String)o["platform"] ?? string.Empty
                }).ToList();
            return members;
        }

        private static NpTitleDetail ParseTitleDetail(JObject o)
        {
            return new NpTitleDetail
            {
                NpTitleIconUrl = (string)o["npTitleIconUrl"] ?? string.Empty,
                NpCommunicationId = (string)o["npCommunicationId"] ?? string.Empty,
                NpTitleName = (string)o["npTitleName"] ?? string.Empty,
                NpTitleId = (string)o["npTitleId"] ?? string.Empty
            };
        }
    }
}
