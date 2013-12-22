using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class SessionInviteEntity
    {
        public class FromUser
        {
            public string OnlineId { get; set; }
        }

        public class NpTitleDetail
        {
            public string NpTitleId { get; set; }
            public string NpCommunicationId { get; set; }
            public string NpTitleName { get; set; }
            public string NpTitleIconUrl { get; set; }
        }

        public class Invitation
        {
            public string InvitationId { get; set; }

            public string Message { get; set; }
            public bool SeenFlag { get; set; }
            public bool UsedFlag { get; set; }
            public string SessionId { get; set; }
            public DateTime ReceivedDate { get; set; }
            public DateTime UpdateDate { get; set; }
            public bool Expired { get; set; }
            public FromUser FromUser { get; set; }
            public List<string> AvailablePlatforms { get; set; }
            public string Subject { get; set; }
            public NpTitleDetail NpTitleDetail { get; set; }
        }

        public class Member
        {
            public string OnlineId { get; set; }
            public string Platform { get; set; }
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

        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }
        public List<Invitation> Invitations { get; set; }

        public static SessionInviteEntity Parse(JObject jobject)
        {
            var sessionInviteEntity = new SessionInviteEntity
            {
                Size = jobject["size"] != null ? (int)jobject["size"] : 0,
                Start = jobject["start"] != null ? (int)jobject["start"] : 0,
                TotalResults = jobject["totalResults"] != null ? (int)jobject["totalResults"] : 0,
                Invitations = jobject["invitations"] != null ? ParseInvitation(jobject) : null
            };
            return sessionInviteEntity;
        }

        private static List<Invitation> ParseInvitation(JObject obj)
        {
            string json = obj["invitations"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var invitations = (from JObject o in a
                select new Invitation
                {
                    InvitationId = o["invitationId"] != null ? (string)o["invitationId"] : string.Empty,
                    Message = o["message"] != null ? (string)o["message"] : string.Empty,
                    SeenFlag = o["seenFlag"] != null && (bool)o["seenFlag"],
                    UsedFlag = o["usedFlag"] != null && (bool)o["usedFlag"],
                    AvailablePlatforms = o["availablePlatforms"] != null ? (from platform in o["availablePlatforms"] select platform.ToString()).ToList() : new List<string>(),
                    SessionId = o["sessionId"] != null ? (string)o["sessionId"] : string.Empty,
                    ReceivedDate = o["receivedDate"] != null ? DateTime.Parse((string)o["receivedDate"]).ToLocalTime() : new DateTime(),
                    UpdateDate = o["updateDate"] != null ? DateTime.Parse((string)o["updateDate"]).ToLocalTime() : new DateTime(),
                    Expired = o["expired"] != null && (bool)o["expired"],
                    FromUser = (JObject)o["fromUser"] != null ? ParseFromUser((JObject)o["fromUser"]) : null,
                    NpTitleDetail = (JObject)o["npTitleDetail"] != null ? ParseTitleDetail((JObject)o["npTitleDetail"]) : null,
                    Subject = o["subject"] != null ? (string)o["subject"] : string.Empty
                }).ToList();
            return invitations;
        }

        private static FromUser ParseFromUser(JObject o)
        {
            var fromUser = new FromUser
            {
                OnlineId = (String)o["onlineId"],
            };
            return fromUser;
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
