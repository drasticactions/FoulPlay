using Microsoft.Phone.Controls;
using PlaystationApp.Resources;

namespace PlaystationApp.Views
{
    public partial class MessagePostView : PhoneApplicationPage
    {
        public MessagePostView()
        {
            InitializeComponent();
            HeaderTextBlock.Text = string.Format(AppResources.ToMessageHeader, App.SelectedUser.OnlineId);
        }
    }
}