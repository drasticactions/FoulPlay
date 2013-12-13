using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PlaystationApp.Views
{
    public partial class LoginConnectPage : PhoneApplicationPage
    {
        private const string ApiString =
            "https://reg.api.km.playstation.net/regcam/mobile/sign-in.html?redirectURL=com.playstation.PlayStationApp://redirect&client_id=4db3729d-4591-457a-807a-1cf01e60c3ac&scope=sceapp";

        public LoginConnectPage()
        {
            InitializeComponent();
            LoginWebBrowser.IsScriptEnabled = false;
            LoginWebBrowser.Navigate(new Uri(ApiString));
        }

        private void LoginWebBrowser_OnNavigated(object sender, NavigationEventArgs e)
        {
            string uri = e.Uri.ToString();
            if (!uri.Equals("https://reg.api.km.playstation.net/regcam/mobile/signin"))
            {
                LoginWebBrowser.IsScriptEnabled = false;
                //MessageBox.Show("完了");
            }
        }
    }
}