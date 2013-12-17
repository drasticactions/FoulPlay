using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using PlaystationApp.Core.Tools;

namespace PlaystationApp.Views
{
    public partial class BatchFriendsPage : PhoneApplicationPage
    {
        public BatchFriendsPage()
        {
            InitializeComponent();
        }

        public static InfiniteScrollingCollection FriendCollection { get; set; }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadingProgressBar.Visibility = Visibility.Visible;            
            var user = App.UserAccountEntity.GetUserEntity();
            var friendManager = new FriendManager();
            FriendCollection = new InfiniteScrollingCollection
            {
                FriendList = new ObservableCollection<FriendsEntity.Friend>(),
                UserAccountEntity = App.UserAccountEntity,
                Offset = 32,
                OnlineFilter = false,
                Requested = true,
                Requesting = false,
                PersonalDetailSharing = false,
                FriendStatus = true
            };
            var items =
                await
                    friendManager.GetFriendsList(user.OnlineId, 0, false, false, false,
                        true, false, true, false, App.UserAccountEntity);
            if (items == null)
            {
                return;
            }
            FriendsMessageTextBlock.Visibility = !items.FriendList.Any() ? Visibility.Visible : Visibility.Collapsed;
            foreach (FriendsEntity.Friend item in items.FriendList)
            {
                FriendCollection.FriendList.Add(item);
            }
            FriendsLongListSelector.ItemRealized += friendList_ItemRealized;
            FriendsLongListSelector.DataContext = FriendCollection;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private const int OffsetKnob = 15;

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
                FriendCollection.LoadFriends(App.UserAccountEntity.GetUserEntity().OnlineId);
            }
        }

        private void FriendSelectionCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}