using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.UserControls
{
    public partial class SendFriendRequestUserControl : UserControl
    {
        public SendFriendRequestUserControl()
        {
            InitializeComponent();
        }

        private async void FriendRequestSendButton_OnClick(object sender, RoutedEventArgs e)
        {
            FriendRequestSendButton.IsEnabled = false;
            NewProgressBar.Visibility = Visibility.Visible;
            var friendManager = new FriendManager();
            await
                friendManager.SendFriendRequest(App.SelectedUser.OnlineId, StatusUpdateBox.Text, App.UserAccountEntity);
            var userManager = new UserManager();
            App.SelectedUser = await userManager.GetUser(App.SelectedUser.OnlineId, App.UserAccountEntity);
            var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
                rootFrame.GoBack();
        }
    }
}