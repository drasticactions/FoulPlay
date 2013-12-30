using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class NotificationEntity
    {

        
        public static NotificationEntity Parse(JObject jobject)
        {
            string json = jobject["notifications"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var notificationEntity = new NotificationEntity()
            {
                Size = jobject["size"] != null ? (int)jobject["size"] : 0,
                Start = jobject["start"] != null ? (int)jobject["start"] : 0,
                TotalResults = jobject["totalResults"] != null ? (int)jobject["totalResults"] : 0
            };
            List<Notification> notifications = (from JObject o in a
                                                select new Notification
                                                {
                                                    NotificationGroup = (String)o["notificationGroup"] ?? string.Empty,
                                                    NotificationId = o["notificationId"] != null ? (long)o["notificationId"] : 0,
                                                    SeenFlag = o["seenFlag"] != null && (Boolean)o["seenFlag"],
                                                    ReceivedDate = (String)o["receivedDate"] ?? string.Empty,
                                                    UpdateDate = (String)o["updateDate"] ?? string.Empty,
                                                    Message = (String)o["message"] ?? string.Empty,
                                                    ActionUrl = (String)o["actionUrl"] ?? string.Empty
                                                }).ToList();
            notificationEntity.Notifications = notifications;
            return notificationEntity;
        }

        public class Notification
        {
            public string NotificationGroup { get; set; }
            public object NotificationId { get; set; }
            public bool SeenFlag { get; set; }
            public string ReceivedDate { get; set; }
            public string UpdateDate { get; set; }
            public string ActionUrl { get; set; }
            public string Message { get; set; }
        }

            public List<Notification> Notifications { get; set; }
            public int Start { get; set; }
            public int Size { get; set; }
            public int TotalResults { get; set; }
        
    }
}
