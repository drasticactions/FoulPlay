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
                Size = (int)jobject["size"],
                Start = (int)jobject["start"],
                TotalResults = (int)jobject["totalResults"]
            };
            List<Notification> notifications = (from JObject o in a
                                                select new Notification
                                                {
                                                    NotificationGroup = (String)o["notificationGroup"],
                                                    NotificationId = (long)o["notificationId"],
                                                    SeenFlag = (Boolean)o["seenFlag"],
                                                    ReceivedDate = (String)o["receivedDate"],
                                                    UpdateDate = (String)o["updateDate"],
                                                    Message = (String)o["message"],
                                                    ActionUrl = (String)o["actionUrl"]
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
