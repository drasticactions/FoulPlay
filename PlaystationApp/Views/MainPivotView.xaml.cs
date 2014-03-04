using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using PlaystationApp.Core.Tools;
using PlaystationApp.Resources;
using PlaystationApp.UserControls;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
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
            var profileButton = new ApplicationBarMenuItem
            {
                Text = AppResources.ProfileHeader
            };
            profileButton.Click += profileButton_Click;
            var logoutButton = new ApplicationBarMenuItem
            {
                Text = AppResources.Logout
            };
            logoutButton.Click += LogOutButton_Click;
            var aboutButton = new ApplicationBarMenuItem()
            {
                Text = "About"
            };
            aboutButton.Click += AboutButton_Click;
            var batchFriendButton = new ApplicationBarMenuItem
            {
                Text = AppResources.BatchFriendInvite
            };
            batchFriendButton.Click += BatchFriendButton_Click;
            //ApplicationBar.MenuItems.Add(batchFriendButton);
            ApplicationBar.MenuItems.Add(profileButton);
            ApplicationBar.MenuItems.Add(logoutButton);
            ApplicationBar.MenuItems.Add(aboutButton);

            ApplicationBar.Buttons.Add(appBarButton);
            ApplicationBar.Buttons.Add(refreshButton);
        }

        private async void profileButton_Click(object sender, EventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var userManager = new UserManager();
            App.SelectedUser = await userManager.GetUser(App.UserAccountEntity.GetUserEntity().OnlineId, App.UserAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            if (App.SelectedUser == null)
            {
                MessageBox.Show(AppResources.GenericError);
                return;
            }
            NavigationService.Navigate(new Uri("/Views/UserPage.xaml", UriKind.Relative));
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            var p = new AboutPrompt {Title = "FoulPlay", VersionNumber = "v.Stable"};
            p.Show("Tim Miller (DrasticActions)", "@innerlogic", "t_miller@outlook.com", @"http://twitter.com/innerlogic");
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

        public async void GetFriendsList(bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
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
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                FriendsMessageTextBlock.Visibility = Visibility.Visible;
                FriendsLongListSelector.DataContext = FriendCollection;
                return;
            }
            FriendsMessageTextBlock.Visibility = Visibility.Collapsed;
            FriendsMessageTextBlock.Visibility = !items.FriendList.Any() ? Visibility.Visible : Visibility.Collapsed;
            foreach (var item in items.FriendList)
            {
                FriendCollection.FriendList.Add(item);
            }
            FriendsLongListSelector.ItemRealized += friendList_ItemRealized;
            FriendsLongListSelector.DataContext = FriendCollection;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void LoadMessages()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var messageManager = new MessageManager();
            MessageGroupEntity message = await messageManager.GetMessageGroup(_user.OnlineId, App.UserAccountEntity);
            MessagesMessageTextBlock.Visibility = message != null && (message.MessageGroups != null && (!message.MessageGroups.Any()))
                ? Visibility.Visible
                : Visibility.Collapsed;
            MessageList.DataContext = message;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void LoadSessionInviteList()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            InviteCollection = new InfiniteScrollingCollection
            {
                Offset = 32,
                InviteCollection = new ObservableCollection<SessionInviteEntity.Invitation>()
            };
            var sessionInvite = new SessionInviteManager();
            var inviteEntity = await sessionInvite.GetSessionInvites(0, App.UserAccountEntity);
            if (inviteEntity == null)
            {
                InviteCollection = null;
                InvitationsLongListSelector.DataContext = InviteCollection;
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                NoInvitesTextBlock.Visibility = Visibility.Visible;
                return;
            }
            if (inviteEntity.Invitations != null && !inviteEntity.Invitations.Any())
            {
                NoInvitesTextBlock.Visibility = Visibility.Visible;
            }
            if (inviteEntity.Invitations != null && inviteEntity.Invitations.Any())
            {
                NoInvitesTextBlock.Visibility = Visibility.Collapsed;
                foreach (var item in inviteEntity.Invitations)
                {
                    InviteCollection.InviteCollection.Add(item);
                }
            }
            InvitationsLongListSelector.DataContext = InviteCollection;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void LoadLiveFromPlaystationList()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var liveStreamManager = new LiveStreamManager();

            // TODO: Remove hardcoded filter list values. Currently this is used for testing.

            var filterList = new Dictionary<string, string> { { "platform", "PS4" }, { "type", "live" }, { "interactive", "true" } };
            var ustreamList = await liveStreamManager.GetUstreamFeed(0, 80, "compact", filterList, "views", string.Empty, App.UserAccountEntity);
            var twitchList = await liveStreamManager.GetTwitchFeed(0, 80, "PS4", "true", string.Empty, App.UserAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            return;
        }

        private async void LoadRecentActivityList()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
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
            if (recentActivityEntity == null)
            {
                RecentActivityCollection = null;
                RecentActivityLongListSelector.DataContext = RecentActivityCollection;
                NoActivitiesTextBlock.Visibility = Visibility.Visible;
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            if (recentActivityEntity.feed != null)
            {
                NoActivitiesTextBlock.Visibility = Visibility.Collapsed;
                foreach (var item in recentActivityEntity.feed)
                {
                    RecentActivityCollection.FeedList.Add(item);
                }
            }

            RecentActivityLongListSelector.DataContext = RecentActivityCollection;
            RecentActivityLongListSelector.ItemRealized += RecentActivity_ItemRealized;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
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
            LoadingProgressBar.Visibility = Visibility.Visible;
            var friend = (FriendsEntity.Friend) FriendsLongListSelector.SelectedItem;
            if (friend == null)
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                return;
            }
            var userManager = new UserManager();
            App.SelectedUser = await userManager.GetUser(friend.OnlineId, App.UserAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            if (App.SelectedUser == null)
            {
                MessageBox.Show(AppResources.GenericError);
                return;
            }
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
                GetFriendsList(filterItem.IsOnline, filterItem.PlayerBlocked, filterItem.PlayedRecently,
                    filterItem.PersonalDetailSharing, filterItem.FriendStatus, filterItem.Requesting,
                    filterItem.Requested);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml", UriKind.Relative));
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshFriendsList();
        }

        private void RefreshFriendsList()
        {
            if (!FilterListPicker.Items.Any()) return;
            var filterItem = (FilterItemEntity)FilterListPicker.SelectedItem;
                GetFriendsList(filterItem.IsOnline, filterItem.PlayerBlocked, filterItem.PlayedRecently,
                    filterItem.PersonalDetailSharing, filterItem.FriendStatus, filterItem.Requesting,
                    filterItem.Requested);
        }


        private void LogOutButton_Click(object sender, EventArgs e)
        {
            _appSettings["refreshToken"] = string.Empty;
            _appSettings["accessToken"] = string.Empty;
            _appSettings.Save();
            NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
        }

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
            var item = (SessionInviteEntity.Invitation) InvitationsLongListSelector.SelectedItem;
            if (item == null) return;
            App.SelectedInvitation = item;
            string url = string.Format("/Views/InvitePage.xaml?inviteId={0}",item.InvitationId);
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void HomePivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (HomePivot.SelectedIndex)
            {
                case 1:
                    LoadMessages();
                    break;
                case 2:
                    LoadSessionInviteList();
                    break;
                case 3:
                     LoadRecentActivityList();
                    break;
            }
        }

        private void LiveFromPlaystationLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}