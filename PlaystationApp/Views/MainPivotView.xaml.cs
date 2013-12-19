using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using PlaystationApp.Core.Tools;
using PlaystationApp.Resources;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace PlaystationApp.Views
{
    public partial class MainPivotView : PhoneApplicationPage
    {
        private const int OffsetKnob = 15;
        private static UserAccountEntity.User _user;

        public MainPivotView()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            _user = App.UserAccountEntity.GetUserEntity();
            FilterListPicker.ItemsSource = BuildFilterItemEntities();
            FilterListPicker.SelectionChanged += FilterListPicker_OnSelectionChanged;
            //Get online friends list by default.
        }
        private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;
        public static InfiniteScrollingCollection FriendCollection { get; set; }

        public static InfiniteScrollingCollection RecentActivityCollection { get; set; }

        public static InfiniteScrollingCollection InviteCollection { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            await LoadRecentActivityList();
            await LoadSessionInviteList();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            FriendsLongListSelector.SelectedItem = null;
            RecentActivityLongListSelector.SelectedItem = null;
            MessageList.SelectedItem = null;
        }

        // Build a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            var appBarButton =
                new ApplicationBarIconButton(new
                    Uri("/Assets/AppBar/feature.search.png", UriKind.Relative)) {Text = AppResources.UserSearch};
            appBarButton.Click += SearchButton_Click;
            var refreshButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/sync.png", UriKind.Relative))
                {
                    Text = AppResources.Refresh
                };
            refreshButton.Click += RefreshButton_Click;
            var logoutButton = new ApplicationBarMenuItem
            {
                Text = AppResources.Logout
            };
            logoutButton.Click += LogOutButton_Click;
            var batchFriendButton = new ApplicationBarMenuItem
            {
                Text = AppResources.BatchFriendInvite
            };
            batchFriendButton.Click += BatchFriendButton_Click;
            //ApplicationBar.MenuItems.Add(batchFriendButton);
            ApplicationBar.MenuItems.Add(logoutButton);
            ApplicationBar.Buttons.Add(appBarButton);
            ApplicationBar.Buttons.Add(refreshButton);
        }

        private void BatchFriendButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/BatchFriendsPage.xaml", UriKind.Relative));
        }

        private void BuildLocalizedPivot()
        {
        }

        private static IEnumerable<FilterItemEntity> BuildFilterItemEntities()
        {
            return new List<FilterItemEntity>
            {
                new FilterItemEntity
                {
                    Name = AppResources.FriendsFilterOnline,
                    FriendStatus = true,
                    PlayedRecently = false,
                    PersonalDetailSharing = false,
                    Requested = false,
                    Requesting = false,
                    IsOnline = true,
                    PlayerBlocked = false
                },
                new FilterItemEntity
                {
                    Name = AppResources.FriendsFilterAll,
                    FriendStatus = true,
                    PersonalDetailSharing = false,
                    Requested = false,
                    Requesting = false,
                    IsOnline = false,
                    PlayerBlocked = false
                },
                new FilterItemEntity
                {
                    Name = AppResources.FriendRequestReceived,
                    FriendStatus = true,
                    PlayedRecently = false,
                    PersonalDetailSharing = false,
                    Requested = true,
                    Requesting = false,
                    IsOnline = false,
                    PlayerBlocked = false
                },
                new FilterItemEntity
                {
                    Name = AppResources.FriendRequestSent,
                    FriendStatus = true,
                    PlayedRecently = false,
                    PersonalDetailSharing = false,
                    Requested = false,
                    Requesting = true,
                    IsOnline = false,
                    PlayerBlocked = false
                },
                new FilterItemEntity
                {
                    Name = AppResources.NameRequestReceived,
                    FriendStatus = false,
                    PlayedRecently = false,
                    PersonalDetailSharing = true,
                    Requested = false,
                    Requesting = true,
                    IsOnline = false,
                    PlayerBlocked = false
                },
                new FilterItemEntity
                {
                    Name = AppResources.NameRequestSent,
                    FriendStatus = false,
                    PlayedRecently = false,
                    PersonalDetailSharing = true,
                    Requested = true,
                    Requesting = false,
                    IsOnline = false,
                    PlayerBlocked = false
                }
            };
        }

        public async Task<bool> GetFriendsList(bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
            bool personalDetailSharing, bool friendStatus, bool requesting, bool requested)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var friendManager = new FriendManager();
            FriendCollection = new InfiniteScrollingCollection
            {
                FriendList = new ObservableCollection<FriendsEntity.Friend>(),
                UserAccountEntity = App.UserAccountEntity,
                Offset = 32,
                OnlineFilter = onlineFilter,
                Requested = requested,
                Requesting = requesting,
                PersonalDetailSharing = personalDetailSharing,
                FriendStatus = friendStatus
            };
            var items =
                await
                    friendManager.GetFriendsList(_user.OnlineId, 0, blockedPlayer, recentlyPlayed, personalDetailSharing,
                        friendStatus, requesting, requested, onlineFilter, App.UserAccountEntity);
            if (items == null)
            {
                return false;
            }
            FriendsMessageTextBlock.Visibility = !items.FriendList.Any() ? Visibility.Visible : Visibility.Collapsed;
            foreach (FriendsEntity.Friend item in items.FriendList)
            {
                FriendCollection.FriendList.Add(item);
            }
            FriendsLongListSelector.ItemRealized += friendList_ItemRealized;
            FriendsLongListSelector.DataContext = FriendCollection;

            var messageManager = new MessageManager();
            //var notificationManager = new NotificationManager();
            //            NotificationEntity notification =
            //    await notificationManager.GetNotifications(_user.OnlineId, App.UserAccountEntity);
            //NotificationsMessageTextBlock.Visibility = !notification.Notifications.Any()
            //    ? Visibility.Visible
            //    : Visibility.Collapsed;
            //NotificationListSelector.DataContext = notification;
            MessageGroupEntity message = await messageManager.GetMessageGroup(_user.OnlineId, App.UserAccountEntity);
            MessagesMessageTextBlock.Visibility = message != null && !message.MessageGroups.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
            MessageList.DataContext = message;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            return true;
        }

        private async Task<bool> LoadSessionInviteList()
        {
            InviteCollection = new InfiniteScrollingCollection
            {
                Offset = 32,
                InviteCollection = new ObservableCollection<SessionInviteEntity.Invitation>()
            };
            var sessionInvite = new SessionInviteManager();
            var inviteEntity = await sessionInvite.GetSessionInvites(0, App.UserAccountEntity);
            if (inviteEntity == null) return false;
            
            foreach (var item in inviteEntity.Invitations)
            {
                InviteCollection.InviteCollection.Add(item);
            }
            InvitationsLongListSelector.DataContext = InviteCollection;
            return true;
        }

        private async Task<bool> LoadRecentActivityList()
        {
            RecentActivityCollection = new InfiniteScrollingCollection
            {
                IsNews = true,
                StorePromo = true,
                UserAccountEntity = App.UserAccountEntity,
                FeedList = new ObservableCollection<RecentActivityEntity.Feed>(),
                PageCount = 1
            };
            var recentActivityManager = new RecentActivityManager();
            var recentActivityEntity =
                await recentActivityManager.GetActivityFeed(_user.OnlineId, 0, true, true, App.UserAccountEntity);
            if (recentActivityEntity == null) return false;
            foreach (var item in recentActivityEntity.FeedList)
            {
                RecentActivityCollection.FeedList.Add(item);
            }

            if (recentActivityEntity.FeedList.Count < 15)
            {
                recentActivityEntity =
                await recentActivityManager.GetActivityFeed(_user.OnlineId, 1, true, true, App.UserAccountEntity);
                if (!recentActivityEntity.FeedList.Any())
                {
                    NoActivitiesTextBlock.Visibility = Visibility.Visible;
                    return false;
                }

                foreach (var item in recentActivityEntity.FeedList)
                {
                    RecentActivityCollection.FeedList.Add(item);
                }

                RecentActivityCollection.PageCount = 2;
            }
           
            RecentActivityLongListSelector.DataContext = RecentActivityCollection;
            RecentActivityLongListSelector.ItemRealized += RecentActivity_ItemRealized;
            return true;
        }

        private void RecentActivity_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (RecentActivityCollection.IsLoading || !RecentActivityCollection.HasMoreItems ||
                RecentActivityLongListSelector.ItemsSource == null ||
                RecentActivityLongListSelector.ItemsSource.Count < OffsetKnob)
                return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var friend = e.Container.Content as RecentActivityEntity.Feed;
            if (friend != null &&
                friend.Equals(
                    RecentActivityLongListSelector.ItemsSource[
                        RecentActivityLongListSelector.ItemsSource.Count - OffsetKnob]))
            {
                RecentActivityCollection.LoadFeedList(_user.OnlineId);
            }
        }

        private async void FriendsLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var friend = (FriendsEntity.Friend) FriendsLongListSelector.SelectedItem;
            if (friend == null) return;
            var userManager = new UserManager();
            App.SelectedUser = await userManager.GetUser(friend.OnlineId, App.UserAccountEntity);
            NavigationService.Navigate(new Uri("/Views/UserPage.xaml", UriKind.Relative));
        }

        private void friendList_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (FriendCollection.IsLoading || !FriendCollection.HasMoreItems ||
                FriendsLongListSelector.ItemsSource == null || FriendsLongListSelector.ItemsSource.Count < OffsetKnob)
                return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var friend = e.Container.Content as FriendsEntity.Friend;
            if (friend != null &&
                friend.Equals(
                    FriendsLongListSelector.ItemsSource[FriendsLongListSelector.ItemsSource.Count - OffsetKnob]))
            {
                FriendCollection.LoadFriends(_user.OnlineId);
            }
        }

        private async void FilterListPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!FilterListPicker.Items.Any()) return;
            var filterItem = (FilterItemEntity) FilterListPicker.SelectedItem;
            if (filterItem == null) return;
            await
                GetFriendsList(filterItem.IsOnline, filterItem.PlayerBlocked, filterItem.PlayedRecently,
                    filterItem.PersonalDetailSharing, filterItem.FriendStatus, filterItem.Requesting,
                    filterItem.Requested);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml", UriKind.Relative));
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await RefreshFriendsList();
        }

        private async Task<bool> RefreshFriendsList()
        {
            if (!FilterListPicker.Items.Any()) return false;
            var filterItem = (FilterItemEntity)FilterListPicker.SelectedItem;
            await
                GetFriendsList(filterItem.IsOnline, filterItem.PlayerBlocked, filterItem.PlayedRecently,
                    filterItem.PersonalDetailSharing, filterItem.FriendStatus, filterItem.Requesting,
                    filterItem.Requested);
            return true;
        }

        private void MessageButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            _appSettings["refreshToken"] = string.Empty;
            _appSettings["accessToken"] = string.Empty;
            _appSettings.Save();
            NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
        }

        //private void NotificationListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var item = (NotificationEntity.Notification) NotificationListSelector.SelectedItem;
        //    if (item == null) return;
        //    if (item.ActionUrl.Contains("NewMessage"))
        //    {
        //        HomePivot.SelectedIndex = 1;
        //    }
        //    if (item.ActionUrl.Contains("personalDetailSharing=requested"))
        //    {
        //        HomePivot.SelectedIndex = 0;
        //    }
        //    if (item.ActionUrl.Contains("friendStatus=requested"))
        //    {
        //        HomePivot.SelectedIndex = 0;
        //    }
        //}

        private void RecentActivityLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (RecentActivityEntity.Feed) RecentActivityLongListSelector.SelectedItem;
            if (item == null) return;
            App.SelectedRecentActivityFeedEntity = item;
            RecentActivityLongListSelector.SelectedItem = null;
            NavigationService.Navigate(new Uri("/Views/RecentActivityPage.xaml", UriKind.Relative));

        }

        private void MessageList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (MessageGroupEntity.MessageGroup) MessageList.SelectedItem;
            if (item == null) return;
            App.SelectedMessageGroupId = item.MessageGroupId;
            MessageList.SelectedItem = null;
            NavigationService.Navigate(new Uri("/Views/MessageView.xaml", UriKind.Relative));
        }

        private void InvitationsLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}