using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PlaystationApp.Views
{
    public partial class SendFriendRequestPage : PhoneApplicationPage
    {
        public SendFriendRequestPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserSearchResultGrid.DataContext = App.SelectedUser;
        }
    }
}