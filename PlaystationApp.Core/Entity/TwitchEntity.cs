using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Entity
{
    public class TwitchEntity
    {
        public class Stream
        {
            public string sce_platform { get; set; }
            public string sce_user_online_id { get; set; }
            public string sce_user_np_id { get; set; }
            public string sce_user_country { get; set; }
            public int sce_title_age_rating { get; set; }
            public string sce_title_language { get; set; }
            public string sce_title_id { get; set; }
            public string sce_title_product_id { get; set; }
            public string sce_title_name { get; set; }
            public string sce_title_short_name { get; set; }
            public string sce_title_store_url { get; set; }
            public string sce_title_genre { get; set; }
            public string sce_title_metadata { get; set; }
            public string sce_title_session_id { get; set; }
            public object sce_title_attribute { get; set; }
            public bool sce_title_preset { get; set; }
            public string sce_title_preset_text_1 { get; set; }
            public string sce_title_preset_text_2 { get; set; }
            public string sce_title_preset_text_3 { get; set; }
            public string sce_title_preset_text_4 { get; set; }
            public string sce_title_preset_text_5 { get; set; }
            public string sce_title_preset_text_description { get; set; }
            public int viewers { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public object broadcast_id { get; set; }
            public string preview { get; set; }
            public string preview_mid { get; set; }
            public string preview_small { get; set; }
            public bool? mature { get; set; }
            public string game { get; set; }
            public string stream_up { get; set; }
        }
            public List<Stream> streams { get; set; }
    }
}
