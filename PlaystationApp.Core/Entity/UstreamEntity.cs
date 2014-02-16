using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Entity
{
    public class UstreamEntity
    {
        public class Thumbnail
        {
            public string live { get; set; }
        }

        public class Picture
        {
            public string href { get; set; }
            public string rel { get; set; }
        }

        public class Owner
        {
            public int id { get; set; }
            public string username { get; set; }
            public Picture picture { get; set; }
        }

        public class Stats
        {
            public int viewer { get; set; }
            public int socialstream { get; set; }
        }

        public class Sce
        {
            public string sce_title_id { get; set; }
            public string sce_user_online_id { get; set; }
            public string sce_title_session_id { get; set; }
            public bool sce_title_preset_text { get; set; }
        }

        public class Media
        {
            public string id { get; set; }
            public Thumbnail thumbnail { get; set; }
            public Owner owner { get; set; }
            public Stats stats { get; set; }
            public string status { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public int stream_started_at { get; set; }
            public Sce sce { get; set; }
        }

        public class Item
        {
            public string href { get; set; }
            public string rel { get; set; }
            public Media media { get; set; }
        }

        public class Actual
        {
            public string href { get; set; }
        }

        public class Next
        {
            public string href { get; set; }
        }

        public class Paging
        {
            public Actual actual { get; set; }
            public Next next { get; set; }
        }

        public List<Item> items { get; set; }
        public Paging paging { get; set; }
    }
}
