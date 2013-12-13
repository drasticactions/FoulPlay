using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace PlaystationApp.Views
{
    public partial class LoginPage : PhoneApplicationPage
    {
        // Constructor
        public LoginPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        //Sample code for building a localized ApplicationBar

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask
            {
                Uri =
                    new Uri(
                        "https://reg.api.km.playstation.net/regcam/mobile/sign-in.html?redirectURL=com.playstation.PlayStationApp://redirect&client_id=4db3729d-4591-457a-807a-1cf01e60c3ac&scope=sceapp")
            };
            task.Show();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }
    }
}