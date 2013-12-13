using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PlaystationApp.Views
{
    public partial class TrophyDetailPage : PhoneApplicationPage
    {
        public TrophyDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ContentPanel.DataContext = App.SelectedTrophyDetail;
        }
    }
}