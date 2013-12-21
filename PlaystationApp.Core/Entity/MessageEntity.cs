using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.Core.Entity
{
    public class MessageEntity
    {
        public class Member
        {
            public string OnlineId { get; set; }
        }

        public class MessageGroupDetail
        {
            public int MessageGroupType { get; set; }
            public string MessageGroupName { get; set; }
            public int TotalMembers { get; set; }
            public List<Member> Members { get; set; }
        }

        public class MessageGroup
        {
            public string MessageGroupId { get; set; }
            public MessageGroupDetail MessageGroupDetail { get; set; }
            public int TotalUnseenMessages { get; set; }
            public int TotalDataUsedMessages { get; set; }
            public int TotalMessages { get; set; }
        }

        public class Message
        {
            public int MessageUid { get; set; }
            public object FakeMessageUid { get; set; }
            public bool SeenFlag { get; set; }
            public bool DataUsedFlag { get; set; }
            public int MessageKind { get; set; }
            public string SenderOnlineId { get; set; }
            public string SentMessageId { get; set; }
            public string ReceivedDate { get; set; }
            public ContentKeys ContentKeys { get; set; }
            public string Body { get; set; }

            public string AvatarUrl { get; set; }
        }

        public class ContentKeys
        {
            public List<string> ContentKeyValues { get; set; }

            public bool HasImage { get; set; }

            public bool HasAudio { get; set; }
        
        }

        public static List<Message> ParseMessage(JObject obj, UserAccountEntity userAccountEntity)
        {
            string json = obj["messages"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var userManager = new UserManager();
             var messages = (from JObject o in a
                            select new Message
                            {
                                MessageUid = o["messageUid"] != null ? (int)o["messageUid"] : 0,
                                FakeMessageUid = o["fakeMessageUid"] != null ? (long)o["fakeMessageUid"] : 0,
                                SeenFlag = o["seenFlag"] != null && (bool)o["seenFlag"],
                                DataUsedFlag = o["dataUsedFlag"] != null && (bool)o["dataUsedFlag"],
                                MessageKind = o["messageKind"] != null ? (int)o["messageKind"] : 0,
                                SenderOnlineId = (String)o["senderOnlineId"] ?? string.Empty,
                                SentMessageId = (String)o["sentMessageId"] ?? string.Empty,
                                ReceivedDate = (String)o["receivedDate"] ?? string.Empty,
                                ContentKeys = o["contentKeys"] != null ? ParseContentKeys(o["contentKeys"].ToString()) : null,
                                Body = (String)o["body"]
                            }).ToList();

            return messages;
        }

        public static ContentKeys ParseContentKeys(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var contentKeys = new ContentKeys
            {
                ContentKeyValues = a.Select(o => o.ToString()).ToList(),
                HasAudio = false
            };
            // TODO: Add Audio Support
            //contentKeys.HasAudio = contentKeys.ContentKeyValues.Contains("voice-data-0");
            contentKeys.HasImage = contentKeys.ContentKeyValues.Contains("image-data-0");
            return contentKeys;
        }

        public MessageGroup MessageGroupEntity { get; set; }
        public List<Message> Messages { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }

        public async static Task<MessageEntity> Parse(JObject jobject, UserAccountEntity userAccountEntity)
        {
            if (jobject["messageGroup"] == null) return new MessageEntity();
            string json = jobject["messageGroup"].ToString();
            JObject messageGroupObject = JObject.Parse(json);
            var messageGroup = new MessageGroup
            {
                TotalMessages = messageGroupObject["totalMessages"] != null ? (int)messageGroupObject["totalMessages"] : 0,
                TotalUnseenMessages = messageGroupObject["totalUnseenMessages"] != null ? (int)messageGroupObject["totalUnseenMessages"] : 0,
                MessageGroupId = messageGroupObject["messageGroupId"] != null ? (String)messageGroupObject["messageGroupId"] : null,
                MessageGroupDetail = messageGroupObject["messageGroupDetail"] != null ? ParseMessageGroupDetail((JObject)messageGroupObject["messageGroupDetail"]) : null
            };
            var memberNames = messageGroup.MessageGroupDetail.Members;
            var messages = ParseMessage(jobject, userAccountEntity);
            var userManager = new UserManager();
            foreach (var member in memberNames)
            {
                var user = await userManager.GetUser(member.OnlineId, userAccountEntity);
                var avatarUrl = user.AvatarUrl;
                foreach (var message in messages)
                {
                    if (message.SenderOnlineId.Equals(user.OnlineId))
                    {
                        message.AvatarUrl = avatarUrl;
                    }
                }
            }
            var messageGroupEntity = new MessageEntity()
            {
                Size = (int)jobject["size"],
                Start = (int)jobject["start"],
                TotalResults = (int)jobject["totalResults"],
                MessageGroupEntity = messageGroup,
                Messages = messages
            };
            return messageGroupEntity;
        }

        public static MessageGroupDetail ParseMessageGroupDetail(JObject o)
        {
            string json = o["members"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
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


    }
}
