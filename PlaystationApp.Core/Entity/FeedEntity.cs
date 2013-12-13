using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class FeedEntity
    {
        public List<Feed> FeedList { get; set; }

        public static FeedEntity Parse(JObject jobject)
        {
            string json = jobject["feed"].ToString();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            
            List<Feed> feedList = (from JObject o in a
                                   select new Feed
                                   {
                                       SmallImageAspectRatio = (String)o["smallImageAspectRatio"],
                                       SmallImageUrl = (String)o["smallImageUrl"],
                                       ServiceProviderName = (String)o["serviceProviderName"],
                                       ServiceProviderImageUrl = (String)o["serviceProviderImageUrl"],
                                       Index = (int)o["index"],
                                       AlwaysShow = (bool)o["alwaysShow"],
                                       Caption = (String)o["caption"],
                                       Block = (int)o["block"],
                                       CaptionTemplate = (String)o["captionTemplate"],
                                       CommentCount = (int)o["commentCount"],
                                       StoryComment = (String)o["storyComment"],
                                       StoryId = (String)o["storyId"],
                                       StoryType = (String)o["storyType"],
                                       Actions = ParseActions(o["actions"].ToString()),
                                       Targets = ParseTarget(o["targets"].ToString()),
                                       Source = ParseSource((JObject)o["source"]),
                                       Params = ParseParam(o["params"].ToString()),
                                       TitleId = (String)o["storyId"],
                                       ThumbnailImageUrl = (String)o["storyId"],
                                       LikeCount = (int)o["storyId"],
                                       LargeImageUrl = (String)o["storyId"],
                                       ProductId = (String)o["storyId"],
                                       ProductUrl = (String)o["storyId"],
                                       Date = (String)o["storyId"],
                                       Liked = (bool)o["liked"],
                                       Relevancy = (double)o["storyId"],
                                       Reshareable = (bool)o["reshareable"],
                                       Start = (int)o["start"]
                                   }).ToList();
            var feedEntity = new FeedEntity()
            {

            };
            return feedEntity;
        }

        public class Target
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
            public string AspectRatio { get; set; }
        }

        private static List<Target> ParseTarget(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            List<Target> targetList = (from JObject o in a
                                       select new Target
                                       {
                                           Meta = (String)o["meta"],
                                           Type = (String)o["type"],
                                           ImageUrl = (String)o["imageUrl"],
                                           AspectRatio = (String)o["aspectRatio"]
                                       }).ToList();
            return targetList;
        }

        public class Source
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        private static Source ParseSource(JObject o)
        {
            return new Source()
            {
                ImageUrl = (String)o["imageUrl"],
                Type = (String)o["type"],
                Meta = (String)o["meta"]
            };
        }

        public class Param
        {
            public string Meta { get; set; }
            public string Type { get; set; }
        }

        private static List<Param> ParseParam(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            List<Param> paramList = (from JObject o in a
                                       select new Param
                                       {
                                           Meta = (String)o["meta"],
                                           Type = (String)o["type"]
                                       }).ToList();
            return paramList;
        }

        public class Action
        {
            public string Type { get; set; }
            public string Uri { get; set; }
            public string Platform { get; set; }
            public string ButtonCaption { get; set; }
            public string ImageUrl { get; set; }
        }

        private static List<Action> ParseActions(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            List<Action> actionList = (from JObject o in a
                                       select new Action
                                       {
                                           Type = (String)o["type"],
                                           Uri = (String)o["uri"],
                                           Platform = (String)o["platform"],
                                           ButtonCaption = (String)o["buttonCaption"],
                                           ImageUrl = (String)o["imageUrl"]
                                       }).ToList();
            return actionList;
        }

        public class Feed
        {
            public string Caption { get; set; }
            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<object> CaptionComponents { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public Source Source { get; set; }
            public string Date { get; set; }
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public bool Liked { get; set; }
            public bool Reshareable { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public string ServiceProviderName { get; set; }
            public int? Index { get; set; }
            public int? Start { get; set; }
            public int? Block { get; set; }
            public bool? AlwaysShow { get; set; }
            public string StoryComment { get; set; }
            public List<Param> Params { get; set; }
            public List<Action> Actions { get; set; }
            public string ServiceProviderImageUrl { get; set; }


        }
    }
}
