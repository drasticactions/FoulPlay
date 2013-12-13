using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;

namespace PlaystationApp
{
    public partial class LaunchPage : PhoneApplicationPage
    {
        public LaunchPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string authCode;
            if (NavigationContext.QueryString.TryGetValue("authCode", out authCode))
            {
                App.UserAccountEntity = new UserAccountEntity();
                var authentication = new AuthenticationManager();
                await authentication.RequestAccessToken(authCode);
                LoginTest();
            }
            else
            {
                LoginTest();
            }
            //var code = NavigationContext.QueryString["authCode"];
        }

        private async void LoginTest()
        {
            App.UserAccountEntity = new UserAccountEntity();
            var authManager = new AuthenticationManager();
            bool loginTest = await authManager.RefreshAccessToken(App.UserAccountEntity);
            if (loginTest)
            {
                UserAccountEntity.User user = await authManager.GetUserEntity(App.UserAccountEntity);
                App.UserAccountEntity.SetUserEntity(user);
                NavigationService.Navigate(new Uri("/Views/MainPivotView.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
            }
        }
    }
}