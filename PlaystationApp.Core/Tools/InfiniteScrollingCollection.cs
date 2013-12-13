using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.Core.Tools
{
    public class InfiniteScrollingCollection : INotifyPropertyChanged
    {
        public bool HasMoreItems { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public int MaxCount { get; set; }
        public int PageCount { get; set; }
        private int _page;
        
        public string Username { get; set; }
        public int Offset;
        public bool OnlineFilter;
        public bool PersonalDetailSharing;
        public bool FriendStatus;
        public bool Requesting;
        public bool Requested;
        public bool RecentlyPlayed;
        public bool BlockedPlayer;
        public bool IsNews;
        public bool StorePromo;
        public UserAccountEntity UserAccountEntity;

        public string Query { get; set; }
        public InfiniteScrollingCollection()
            : base()
        {
            HasMoreItems = true;
            IsLoading = false;
            MaxCount = 0;
            OnlineFilter = false;
            this._page = 0;
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;

            }

            private set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public ObservableCollection<FriendsEntity.Friend> FriendList
        {
            get;
            set;
        }
 
        public ObservableCollection<TrophyEntity.TrophyTitle> TrophyList
        { get; set; }

        public ObservableCollection<RecentActivityEntity.Feed> FeedList
        { get; set; }


        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async void LoadFeedList(string username)
        {
            
            IsLoading = true;
            var recentActivityManager = new RecentActivityManager();
            var feedEntity =
                await recentActivityManager.GetActivityFeed(username, PageCount, StorePromo, IsNews, UserAccountEntity);
            foreach (var feed in feedEntity.FeedList)
            {
                FeedList.Add(feed);
            }
            if (feedEntity.FeedList.Any())
            {
                HasMoreItems = true;
                PageCount++;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }

        public async void LoadTrophies(string username)
        {
            Offset = Offset + MaxCount;
            IsLoading = true;
            var trophyManager = new TrophyManager();
            var trophyList = await trophyManager.GetTrophyList(username, Offset, UserAccountEntity);
            foreach (var trophy in trophyList.TrophyTitles)
            {
                TrophyList.Add(trophy);
            }
            if (trophyList.TrophyTitles.Any())
            {
                HasMoreItems = true;
                MaxCount += 64;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }

        public async void LoadFriends(string username)
        {
            
            IsLoading = true;
            var friendManager = new FriendManager();
            var friendEntity = await friendManager.GetFriendsList(username, Offset, BlockedPlayer, RecentlyPlayed, PersonalDetailSharing, FriendStatus, Requesting, Requested, OnlineFilter, UserAccountEntity);
            foreach (var friend in friendEntity.FriendList)
            {
                FriendList.Add(friend);
            }
            if (friendEntity.FriendList.Any())
            {
                HasMoreItems = true;
                Offset = Offset += 32;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}
