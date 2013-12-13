using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationApp.Core.Entity
{
    public class FilterItemEntity
    {
        public string Name { get; set; }

        public bool IsOnline { get; set; }

        public bool PlayedRecently { get; set; }

        public bool PlayerBlocked { get; set; }

        public bool PersonalDetailSharing { get; set; }
        public bool FriendStatus { get; set; }
        public bool Requesting { get; set; }
        public bool Requested { get; set; }
    }
}
