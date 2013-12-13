using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.Core.Entity
{
    public class MessageGroupEntity
    {

        public static async Task<MessageGroupEntity> Parse(JObject jobject, UserAccountEntity userAccountEntity)
        {
            string json = jobject["messageGroups"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            List<MessageGroup> messages = new List<MessageGroup>();
            foreach (JObject o in a)
                messages.Add(new MessageGroup
                {
                    TotalMessages = (int) o["totalMessages"], TotalUnseenMessages = (int) o["totalUnseenMessages"], MessageGroupId = (String) o["messageGroupId"], MessageGroupDetail = await ParseMessageGroupDetail((JObject) o["messageGroupDetail"], userAccountEntity), LatestMessage = await ParseLatestMessage((JObject) o["latestMessage"], userAccountEntity)
                });

            var messageGroupEntity = new MessageGroupEntity()
            {
                Size = (int)jobject["size"],
                Start = (int)jobject["start"],
                TotalResults = (int)jobject["totalResults"],
                MessageGroups = messages
            };
            return messageGroupEntity;
        }

        public static async Task<MessageGroupDetail> ParseMessageGroupDetail(JObject o, UserAccountEntity userAccountEntity)
        {
            string json = o["members"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var userManager = new UserManager();
            List<Member> members = (from JObject q in a
                                    select new Member
                                    {
                                        OnlineId = (String)q["onlineId"],
                                    }).ToList();
            var messageGroupDetail = new MessageGroupDetail()
            {
                MessageGroupName = (String)o["messageGroupName"],
                MessageGroupType = (int)o["messageGroupType"],
                TotalMembers = (int)o["totalMembers"],
                Members = members
            };
            return messageGroupDetail;
        }

        public async static Task<LatestMessage> ParseLatestMessage(JObject o, UserAccountEntity userAccountEntity)
        {
            var userManager = new UserManager();
            var latestMessage = new LatestMessage()
            {
                MessageUid = (int)o["messageUid"],
                SeenFlag = (bool)o["seenFlag"],
                MessageKind = (int)o["messageKind"],
                SenderOnlineId = (String)o["senderOnlineId"],
                ReceivedDate = (String)o["recievedDate"],
                Body = (String)o["body"]
            };
            latestMessage.User = await userManager.GetUser(latestMessage.SenderOnlineId, userAccountEntity);
            
            return latestMessage;

        }

        public class Member
        {
            public string OnlineId { get; set; }

            public string AvatarUrl { get; set; }
        }

        public class MessageGroupDetail
        {
            public int MessageGroupType { get; set; }
            public string MessageGroupName { get; set; }
            public int TotalMembers { get; set; }
            public List<Member> Members { get; set; }
        }

        public class LatestMessage
        {
            public int MessageUid { get; set; }
            public bool SeenFlag { get; set; }
            public int MessageKind { get; set; }
            public string SenderOnlineId { get; set; }
            public string ReceivedDate { get; set; }
            public string Body { get; set; }
            public UserEntity User { get; set; }
        }

        public class MessageGroup
        {
            public string MessageGroupId { get; set; }
            public MessageGroupDetail MessageGroupDetail { get; set; }
            public int TotalUnseenMessages { get; set; }
            public int TotalMessages { get; set; }
            public LatestMessage LatestMessage { get; set; }
        }

        public List<MessageGroup> MessageGroups { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }

    }
}
