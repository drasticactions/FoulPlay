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
            public string onlineId { get; set; }
        }

        public class MessageGroupDetail
        {
            public int messageGroupType { get; set; }
            public string messageGroupName { get; set; }
            public int totalMembers { get; set; }
            public List<Member> members { get; set; }
        }

        public class MessageGroup
        {
            public string messageGroupId { get; set; }
            public string messageGroupModifiedDate { get; set; }
            public MessageGroupDetail messageGroupDetail { get; set; }
            public int totalUnseenMessages { get; set; }
            public int totalDataUsedMessages { get; set; }
            public int totalMessages { get; set; }
        }

        public class Message
        {
            public int messageUid { get; set; }
            public object fakeMessageUid { get; set; }
            public bool seenFlag { get; set; }
            public bool dataUsedFlag { get; set; }
            public int messageKind { get; set; }
            public string senderOnlineId { get; set; }
            public string sentMessageId { get; set; }
            public string receivedDate { get; set; }
            public List<string> contentKeys { get; set; }
            public string body { get; set; }

            public UserEntity user { get; set; }
        }

        public MessageGroup messageGroup { get; set; }
        public List<Message> messages { get; set; }
        public int start { get; set; }
        public int size { get; set; }
        public int totalResults { get; set; }

    }
}
