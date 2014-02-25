using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class RecentActivityEntity
    {
        public class Target
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        public class CondensedStory
        {
            public string Caption { get; set; }
            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<CaptionComponent> CaptionComponents { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public Source Source { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public DateTime Date { get; set; }
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public bool Liked { get; set; }
            public string ServiceProviderName { get; set; }
            public bool Reshareable { get; set; }
        }

        public class CaptionComponent
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Source
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        public class Feed
        {
            public string Caption { get; set; }

            public List<CondensedStory> CondensedStories { get; set; }

            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<CaptionComponent> CaptionComponents { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public Source Source { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public DateTime Date { get; set; }
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public bool Liked { get; set; }
            public string ServiceProviderName { get; set; }
            public bool Reshareable { get; set; }
        }
        public List<Feed> feed { get; set; }

        public static RecentActivityEntity Parse(string json)
        {
            var recentActivity = new RecentActivityEntity();
            var a = (JArray)JsonConvert.DeserializeObject(json);
            recentActivity.feed = (from JObject o in a
                select new Feed
                {
                    Caption = (String)o["caption"] ?? string.Empty,
                    CaptionTemplate = (String)o["captionTemplate"] ?? string.Empty,
                    StoryId = (String)o["storyId"] ?? string.Empty,
                    StoryType = (String)o["storyType"] ?? string.Empty,
                    SmallImageUrl = (String)o["smallImageUrl"] ?? string.Empty,
                    SmallImageAspectRatio = (String)o["smallImageAspectRadio"] ?? string.Empty,
                    LargeImageUrl = (String)o["largeImageUrl"] ?? string.Empty,
                    ThumbnailImageUrl = (String)o["thumbnailImageUrl"] ?? string.Empty,
                    Date = o["date"] != null ? DateTime.Parse((string)o["date"]).ToLocalTime() : new DateTime(),
                    Relevancy = o["relevancy"] != null ? (Double)o["relevancy"] : 0,
                    CommentCount = o["commentCount"] != null ? (int)o["commentCount"] : 0,
                    LikeCount = o["likeCount"] != null ? (int)o["likeCount"] : 0,
                    TitleId = (String)o["titleId"] ?? string.Empty,
                    ProductId = (String)o["productId"] ?? string.Empty,
                    ProductUrl = (String)o["productUrl"] ?? string.Empty,
                    Liked = o["liked"] != null && (bool)o["liked"],
                    ServiceProviderName = (String)o["serviceProviderName"] ?? string.Empty,
                    Reshareable = o["reshareable"] != null && (bool)o["reshareable"],
                    Source = o["source"] != null ? ParseSource((JObject)o["source"]) : null,
                    Targets = o["targets"] != null ? ParseTargets(o["targets"].ToString()) : null,
                    CaptionComponents = o["captionComponents"] != null ? ParseCaptionComponents(o["captionComponents"].ToString()) : null,
                    CondensedStories = o["condensedStories"] != null ? ParseCondensedStory(o["condensedStories"].ToString()) : null
                }).ToList();
            return recentActivity;
        }

        private static List<CondensedStory> ParseCondensedStory(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var condencedStory = (from JObject o in a
                select new CondensedStory()
                {
                    Caption = (String)o["caption"] ?? string.Empty,
                    CaptionTemplate = (String)o["captionTemplate"] ?? string.Empty,
                    StoryId = (String)o["storyId"] ?? string.Empty,
                    StoryType = (String)o["storyType"] ?? string.Empty,
                    SmallImageUrl = (String)o["smallImageUrl"] ?? string.Empty,
                    SmallImageAspectRatio = (String)o["smallImageAspectRadio"] ?? string.Empty,
                    LargeImageUrl = (String)o["largeImageUrl"] ?? string.Empty,
                    ThumbnailImageUrl = (String)o["thumbnailImageUrl"] ?? string.Empty,
                    Date = o["date"] != null ? DateTime.Parse((string)o["date"]).ToLocalTime() : new DateTime(),
                    Relevancy = o["relevancy"] != null ? (Double)o["relevancy"] : 0,
                    CommentCount = o["commentCount"] != null ? (int)o["commentCount"] : 0,
                    LikeCount = o["likeCount"] != null ? (int)o["likeCount"] : 0,
                    TitleId = (String)o["titleId"] ?? string.Empty,
                    ProductId = (String)o["productId"] ?? string.Empty,
                    ProductUrl = (String)o["productUrl"] ?? string.Empty,
                    Liked = o["liked"] != null && (bool)o["liked"],
                    ServiceProviderName = (String)o["serviceProviderName"] ?? string.Empty,
                    Reshareable = o["reshareable"] != null && (bool)o["reshareable"],
                    Source = o["source"] != null ? ParseSource((JObject)o["source"]) : null,
                    Targets = o["targets"] != null ? ParseTargets(o["targets"].ToString()) : null,
                    CaptionComponents = o["captionComponents"] != null ? ParseCaptionComponents(o["captionComponents"].ToString()) : null
                }).ToList();
            return condencedStory;
        }

        private static Source ParseSource(JObject o)
        {
            return new Source
            {
                Meta = (String)o["meta"] ?? string.Empty,
                Type = (String)o["type"] ?? string.Empty,
                ImageUrl = (String)o["imageUrl"] ?? string.Empty
            };
        }

        private static List<Target> ParseTargets(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var targets = (from JObject o in a
                select new Target
                {
                    Meta = (String)o["meta"] ?? string.Empty,
                    Type = (String)o["type"] ?? string.Empty,
                    ImageUrl = (String)o["imageUrl"] ?? string.Empty
                }).ToList();
            return targets;
        }

        private static List<CaptionComponent> ParseCaptionComponents(string json)
        {
            var a = (JArray)JsonConvert.DeserializeObject(json);
            var captionComponents = (from JObject o in a
                select new CaptionComponent
                {
                    Key = (String)o["key"] ?? string.Empty,
                    Value = (String)o["value"] ?? string.Empty,
                }).ToList();
            return captionComponents;
        }

    }
}
