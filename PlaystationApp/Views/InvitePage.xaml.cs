using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.Views
{
    public partial class InvitePage : PhoneApplicationPage
    {
        public InvitePage()
        {
            InitializeComponent();
        }

        private UserEntity _user;

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadingProgressBar.Visibility = Visibility.Visible;
            if (App.SelectedInvitation != null)
            {
                SessionStateGrid.DataContext = App.SelectedInvitation;
                var userManager = new UserManager();
                _user = await userManager.GetUser(App.SelectedInvitation.FromUser.OnlineId, App.UserAccountEntity);
                UserInformationGrid.DataContext = _user;
            }
            string parameterValue = NavigationContext.QueryString["inviteId"];
            var sessionInviteManager = new SessionInviteManager();
            var sessionInvite = await sessionInviteManager.GetInviteInformation(parameterValue, App.UserAccountEntity);
            UserMessageBlock.Text = sessionInvite.Message;
            InviteInformationViewer.DataContext = sessionInvite.session;
            if (sessionInvite.Expired)
            {
                InviteExpiredTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                InviteInformationViewer.Visibility = Visibility.Visible;
                if (sessionInvite.session != null)
                {
                    foreach (var member in sessionInvite.session.Members)
                    {
                        var userManager = new UserManager();
                        var test = await userManager.GetUserAvatar(member.OnlineId, App.UserAccountEntity);
                        member.AvatarUrl = test.AvatarUrl;
                    }
                    PlayersListSelector.ItemsSource = sessionInvite.session.Members;
                }
            }
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void SendMessageToUserButton_OnClick(object sender, RoutedEventArgs e)
        {
            App.SelectedUser = _user;
            NavigationService.Navigate(new Uri("/Views/UserPage.xaml", UriKind.Relative));
        }
    }
}