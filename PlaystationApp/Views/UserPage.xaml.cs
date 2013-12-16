using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
    public partial class UserPage
    {
        private const int OffsetKnob = 15;
        private MessagePrompt _messpagePrompt;

        public UserPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            var refreshButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/sync.png", UriKind.Relative))
                {
                    Text = AppResources.Refresh
                };
            refreshButton.Click += RefreshButton_Click;

            ApplicationBar.Buttons.Add(refreshButton);
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await RefreshGroupMessages();
        }

        private async Task<bool> RefreshGroupMessages()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var messagerManager = new MessageManager();
            _messageEntity = await
                   messagerManager.GetGroupConversation(string.Format("~{0},{1}", App.SelectedUser.OnlineId, App.UserAccountEntity.GetUserEntity().OnlineId), App.UserAccountEntity);
            MessageList.DataContext = _messageEntity;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            return true;
        }

        public static InfiniteScrollingCollection TrophyCollection { get; set; }

        public static InfiniteScrollingCollection RecentActivityCollection { get; set; }

        private async Task<bool> LoadRecentActivityList()
        {
            RecentActivityCollection = new InfiniteScrollingCollection
            {
                IsNews = false,
                StorePromo = true,
                UserAccountEntity = App.UserAccountEntity,
                FeedList = new ObservableCollection<RecentActivityEntity.Feed>(),
                PageCount = 1
            };
            var recentActivityManager = new RecentActivityManager();
            RecentActivityEntity recentActivityEntity =
                await
                    recentActivityManager.GetActivityFeed(App.SelectedUser.OnlineId, 0, true, false,
                        App.UserAccountEntity);
            if (recentActivityEntity == null) return false;
            foreach (RecentActivityEntity.Feed item in recentActivityEntity.FeedList)
            {
                RecentActivityCollection.FeedList.Add(item);
            }
            //App.SelectedRecentActivityEntity = RecentActivityCollection.FeedList;
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
                RecentActivityCollection.LoadFeedList(App.UserAccountEntity.GetUserEntity().OnlineId);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            if (App.SelectedUser.presence == null ||
                (App.SelectedUser.presence != null && App.SelectedUser.presence.PrimaryInfo.GameTitleInfo == null))
            {
                NowPlayingStackPanel.Visibility = Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(App.SelectedUser.AboutMe))
            {
                AboutMeStackPanel.Visibility = Visibility.Collapsed;
            }
            ComparedUserGrid.DataContext = App.SelectedUser;
            FromUserGrid.DataContext = App.UserAccountEntity.GetUserEntity();
            var languageList = App.SelectedUser.LanguagesUsed.Select(ParseLanguageVariable).ToList();
            MyLanguagesBlock.Text = string.Join("," + Environment.NewLine, languageList);
            ProfileGrid.DataContext = App.SelectedUser;
            SetFriendButtons();
            await LoadRecentActivityList();
            if (!RecentActivityCollection.FeedList.Any())
            {
                NoActivitiesTextBlock.Visibility = Visibility.Visible;
                NoActivitiesListTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                RecentActivitiesGrid.Visibility = Visibility.Visible;
                RecentActivityStackPanel.DataContext = RecentActivityCollection.FeedList.FirstOrDefault();
            }
            RecentActivityProgressBar.Visibility = Visibility.Collapsed;
            await GetTrophyList();
            var messagerManager = new MessageManager();
            _messageEntity = await
                   messagerManager.GetGroupConversation(string.Format("~{0},{1}", App.SelectedUser.OnlineId, App.UserAccountEntity.GetUserEntity().OnlineId), App.UserAccountEntity);
            MessageList.DataContext = _messageEntity;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private MessageEntity _messageEntity;

        private async Task<bool> GetTrophyList()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var trophyManager = new TrophyManager();
            TrophyCollection = new InfiniteScrollingCollection
            {
                UserAccountEntity = App.UserAccountEntity,
                TrophyList = new ObservableCollection<TrophyEntity.TrophyTitle>(),
                Offset = 64
            };
            var items = await trophyManager.GetTrophyList(App.SelectedUser.OnlineId, 0, App.UserAccountEntity);
            foreach (TrophyEntity.TrophyTitle item in items.TrophyTitles)
            {
                TrophyCollection.TrophyList.Add(item);
            }
            if (!items.TrophyTitles.Any())
            {
                NoTrophyTextBlock.Visibility = Visibility.Visible;
                TrophyHeaderGrid.Visibility = Visibility.Collapsed;
            }
            TrophyList.ItemRealized += trophyList_ItemRealized;
            TrophyList.DataContext = TrophyCollection;
            return true;
        }

        private void trophyList_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (TrophyCollection.IsLoading || !TrophyCollection.HasMoreItems ||
                TrophyList.ItemsSource == null || TrophyList.ItemsSource.Count < OffsetKnob)
                return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var friend = e.Container.Content as TrophyEntity.TrophyTitle;
            if (friend != null && friend.Equals(TrophyList.ItemsSource[TrophyList.ItemsSource.Count - OffsetKnob]))
            {
                TrophyCollection.LoadTrophies(App.SelectedUser.OnlineId);
            }
        }

        private static string ParseLanguageVariable(string language)
        {
            switch (language)
            {
                case "ja":
                    return AppResources.LangJapanese;
                case "dk":
                    return AppResources.LangDanish;
                case "de":
                    return AppResources.LangGerman;
                case "en":
                    return AppResources.LangEnglishUS;
                case "en-GB":
                    return AppResources.LangEnglishUK;
                case "fi":
                    return AppResources.LangFinnish;
                case "fr":
                    return AppResources.LangFrench;
                case "es":
                    return AppResources.LangSpanishSpain;
                case "es-MX":
                    return AppResources.LangSpanishLA;
                case "it":
                    return AppResources.LangItalian;
                case "nl":
                    return AppResources.LangDutch;
                case "pt":
                    return AppResources.LangPortuguesePortugal;
                case "pt-BR":
                    return AppResources.LangPortugueseBrazil;
                case "ru":
                    return AppResources.LangRussian;
                case "pl":
                    return AppResources.LangPolish;
                case "no":
                    return AppResources.LangNorwegian;
                case "sv":
                    return AppResources.LangSwedish;
                case "tr":
                    return AppResources.LangTurkish;
                case "ko":
                    return AppResources.LangKorean;
                case "zh-CN":
                    return AppResources.LangChineseSimplified;
                case "zh-TW":
                    return AppResources.LangChineseTraditional;
                default:
                    return null;
            }
        }

        private async void SetFriendButtons()
        {
            FriendRequestButton.Visibility = App.SelectedUser.Relation.Contains("friend")
                ? Visibility.Collapsed
                : Visibility.Visible;

            if (App.SelectedUser.Relation.Equals("requested friend"))
            {
                FriendAcceptButton.Visibility = Visibility.Visible;
                var friendManager = new FriendManager();
                App.SelectedFriendRequestMessage =
                    await friendManager.GetRequestMessage(App.SelectedUser.OnlineId, App.UserAccountEntity);
            }
            else FriendAcceptButton.Visibility = Visibility.Collapsed;

            CancelFriendRequestButton.Visibility = App.SelectedUser.Relation.Equals("requesting friend")
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (App.SelectedUser.personalDetail == null ||
                string.IsNullOrEmpty(App.SelectedUser.personalDetail.FirstName)) return;
            FriendRequestButton.Visibility = Visibility.Collapsed;
            if (string.IsNullOrEmpty(App.SelectedUser.personalDetail.ProfilePictureUrl)) return;
            var bm =
                new BitmapImage(new Uri(App.SelectedUser.personalDetail.ProfilePictureUrl, UriKind.RelativeOrAbsolute));
            UserImage.Source = bm;
        }

        private void UserTrophyGrid_OnTap(object sender, GestureEventArgs e)
        {
            UserPivot.SelectedIndex = 3;
        }

        private void SendMessageButton_OnTap(object sender, GestureEventArgs e)
        {
            App.SelectedMessageEventEntity = _messageEntity;
            NavigationService.Navigate(new Uri("/Views/MessagePostView.xaml", UriKind.Relative));
        }

        private void FriendRequestButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SendFriendRequestPage.xaml", UriKind.Relative));
        }

        private void TrophyList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var trophyEntity = (TrophyEntity.TrophyTitle) TrophyList.SelectedItem;
            if (trophyEntity == null) return;
            App.SelectedTrophyTitle = trophyEntity;
            NavigationService.Navigate(new Uri("/Views/TrophyPage.xaml", UriKind.Relative));
        }

        private void RecentActivityLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (RecentActivityEntity.Feed) RecentActivityLongListSelector.SelectedItem;
            if (item == null) return;
            App.SelectedRecentActivityFeedEntity = item;
            NavigationService.Navigate(new Uri("/Views/RecentActivityPage.xaml", UriKind.Relative));
        }

        private void FriendAcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            _messpagePrompt = new MessagePrompt
            {
                Title = AppResources.FriendRequest,
                Body = new AcceptFriendUserControl(),
                IsCancelVisible = false
            };
            var yesButton = new Button {Content = AppResources.AddAsFriend, FontSize = 15.0};
            yesButton.Click += FriendYesButton_click;
            var noButton = new Button {Content = AppResources.DeleteFriendRequest, FontSize = 15.0};
            noButton.Click += FriendNoButton_click;
            _messpagePrompt.ActionPopUpButtons.Clear();
            _messpagePrompt.ActionPopUpButtons.Add(yesButton);
            _messpagePrompt.ActionPopUpButtons.Add(noButton);
            _messpagePrompt.Show();
        }

        private async void FriendNoButton_click(object sender, RoutedEventArgs e)
        {
            if (_messpagePrompt != null)
                _messpagePrompt.Hide();
            LoadingProgressBar.Visibility = Visibility.Visible;
            FriendAcceptButton.IsEnabled = false;
            CancelFriendRequestButton.IsEnabled = false;
            var friendManager = new FriendManager();
            var test = await friendManager.DenyAddFriend(true, App.SelectedUser.OnlineId, App.UserAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            if (!test)
            {
                MessageBox.Show(AppResources.GenericError);
                return;
            }
            FriendAcceptButton.Visibility = Visibility.Collapsed;
            CancelFriendRequestButton.Visibility = Visibility.Collapsed;
        }

        private async void FriendYesButton_click(object sender, RoutedEventArgs e)
        {
            if (_messpagePrompt != null)
                _messpagePrompt.Hide();
            LoadingProgressBar.Visibility = Visibility.Visible;
            FriendAcceptButton.IsEnabled = false;
            var friendManager = new FriendManager();
            bool test = await friendManager.DenyAddFriend(false, App.SelectedUser.OnlineId, App.UserAccountEntity);
            //await RefreshUserData();
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            if (!test)
            {
                MessageBox.Show(AppResources.GenericError);
                return;
            }
            FriendAcceptButton.Visibility = Visibility.Collapsed;
        }

        private void NameRequestAcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CancelFriendRequestButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CancelNameRequstButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void NameRequestButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(AppResources.RealNameRequest, AppResources.NameRequestAlert, MessageBoxButton.OKCancel) !=
                MessageBoxResult.OK) return;
            LoadingProgressBar.Visibility = Visibility.Visible;
            //NameRequestButton.IsEnabled = false;
            var friendManager = new FriendManager();
            bool test = await friendManager.SendNameRequest(App.SelectedUser.OnlineId, App.UserAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            //NameRequestAcceptButton.Visibility = Visibility.Collapsed;
            if (test) return;
            MessageBox.Show(AppResources.NameRequestSendError);
        }

        private void RecentActivitiesGrid_OnTap(object sender, GestureEventArgs e)
        {
            UserPivot.SelectedIndex = 2;
        }

        private async void MessageList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (MessageEntity.Message)MessageList.SelectedItem;
            if (item == null) return;
            var messageManager = new MessageManager();
            if (item.ContentKeys.HasImage)
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                App.SelectedMessage = item;
                var imageBytes = await
                    messageManager.GetMessageContent(_messageEntity.MessageGroupEntity.MessageGroupId, item,
                        App.UserAccountEntity);
                App.SelectedMessageImage = DecodeImage(imageBytes);
                var messagePrompt = new MessagePrompt { Title = AppResources.Image, Body = new MessageImageUserControl() };
                messagePrompt.Show();
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
            else if (item.ContentKeys.HasAudio)
            {
                // TODO: Add audio support
            }
            MessageList.SelectedItem = null;
        }

        public BitmapImage DecodeImage(Stream array)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(array);
            return bitmapImage;
        }    
    }
}