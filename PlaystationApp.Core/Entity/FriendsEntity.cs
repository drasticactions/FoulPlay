using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlaystationApp.Core.Entity
{
    public class FriendsEntity
    {
        public List<Friend> FriendList { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }

        public class EarnedTrophies
        {
            public int Platinum { get; set; }
            public int Gold { get; set; }
            public int Silver { get; set; }
            public int Bronze { get; set; }
        }

        public class Friend
        {
            public string OnlineId { get; set; }
            public string AvatarUrl { get; set; }

            public Boolean Plus { get; set; }
            public TrophySummary TrophySummary { get; set; }
            public Presence Presence { get; set; }
            public PersonalDetail PersonalDetail { get; set; }
        }

        public class GameTitleInfo
        {
            public string NpTitleId { get; set; }
            public string TitleName { get; set; }
        }

        public class PersonalDetail
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName { get; set; }
            public string ProfilePictureUrl { get; set; }
        }

        public class Presence
        {
            public PrimaryInfo PrimaryInfo { get; set; }
        }

        public class PrimaryInfo
        {
            public string Platform { get; set; }
            public string OnlineStatus { get; set; }
            public bool IsOnline { get; set; }
            public GameTitleInfo GameTitleInfo { get; set; }
            public string GameStatus { get; set; }
            public DateTime LastOnlineDate { get; set; }
        }

        public class TrophySummary
        {
            public int Level { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }
        }
    }
}