#define DEBUG_AGENT
using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
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

        private PeriodicTask periodicTask;
        private string periodicTaskName = "PeriodicAgent";
        public bool agentsAreEnabled = true;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string authCode;
            StartPeriodicAgent();
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

        private void StartPeriodicAgent()
        {
            agentsAreEnabled = true;
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName) {Description = "test"};

            try
            {
                ScheduledActionService.Add(periodicTask);
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The Action is disabled"))
                {
                    MessageBox.Show("Background Agents have been disabled!");
                    agentsAreEnabled = false;
                }
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
                
            }
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