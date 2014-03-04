using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Entity
{
    public class LiveStreamEntity
    {
        public int TotalViewers { get; set; }

        public string GameTitle { get; set; }

        public string GameBoxUrl { get; set; }

        public string GameUrl { get; set; }

        public string StreamUrl { get; set; }

        public string StreamTitle { get; set; }

        public string StreamDescription { get; set; }

        public string Thumbnail { get; set; }

        public bool IsStreamLive { get; set; }

        public bool IsTwitch { get; set; }

        public int SocialStream { get; set; }

        public string UserAvatar { get; set; }

        public string Username { get; set; }

        public void Parse(UstreamEntity.Item item)
        {
            this.TotalViewers = item.media.stats.viewer;
        }

    }
}
