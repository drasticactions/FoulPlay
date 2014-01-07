//#define DEBUG_AGENT
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using PlaystationApp.Resources;

namespace PlaystationApp
{
    public partial class LaunchPage : PhoneApplicationPage
    {
        
        public LaunchPage()
        {
            InitializeComponent();
        }

        private PeriodicTask _periodicTask;
        private const string PeriodicTaskName = "PeriodicAgent";
        public bool AgentsAreEnabled = true;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ShellTile iconicTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (iconicTile != null)
            {
                var tileData = new FlipTileData
                {
                    Title = "FoulPlay",
                    Count = 0
                };
                iconicTile.Update(tileData);
            }
            string authCode;
            /*
             * If we're coming from IE with our Auth Code, parse it out and get the Access Token.
             * Else check if we already have it.
             * */
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

        /// <summary>
        /// Sample code for handling a periodic agent.
        /// </summary>
        private void StartPeriodicAgent()
        {
            AgentsAreEnabled = true;
            _periodicTask = ScheduledActionService.Find(PeriodicTaskName) as PeriodicTask;

            if (_periodicTask != null)
            {
                RemoveAgent(PeriodicTaskName);
            }

            _periodicTask = new PeriodicTask(PeriodicTaskName) {Description = AppResources.NotificationsHeader};

            try
            {
                ScheduledActionService.Add(_periodicTask);
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(PeriodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The Action is disabled"))
                {
                    MessageBox.Show("Background Agents have been disabled!");
                    AgentsAreEnabled = false;
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

        /// <summary>
        /// Checks if the user has an authenication token. If we do, load the user and go to the
        /// main page. If not, load the login screen and redirect them to IE.
        /// </summary>
        private async void LoginTest()
        {
            App.UserAccountEntity = new UserAccountEntity();
            var authManager = new AuthenticationManager();
            bool loginTest = await authManager.RefreshAccessToken(App.UserAccountEntity);
            if (loginTest)
            {
                // We have a token! Start the background processing and clear all old notifications.
                StartPeriodicAgent();

                UserAccountEntity.User user = await authManager.GetUserEntity(App.UserAccountEntity);
                App.UserAccountEntity.SetUserEntity(user);

                // Clears old notifications and resets the live tile.
                // Note that this ONLY clears normal notifications. Flags on the individual objects (Like friends
                // or new messages) are still there and are cleared when the user activates them.
                NotificationEntity notificationEntity = await GetNotifications(App.UserAccountEntity);
                var notificationList = notificationEntity.Notifications.Where(o => o.SeenFlag == false);
                foreach (var notification in notificationList)
                {
                    await NotificationManager.ClearNotification(notification, App.UserAccountEntity);
                    
                }

                // remove secondary tile
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("Title=FoulPlay"));
                if (tile != null) tile.Delete();
                NavigationService.Navigate(new Uri("/Views/MainPivotView.xaml", UriKind.Relative));
            }
            else
            {
                // We don't have a token, or something is wrong with their servers :(. Go to the login page...
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Gets and checks normal notifications.
        /// </summary>
        /// <param name="userAccountEntity">The user account entity.</param>
        /// <returns>A Notification Entity</returns>
        private async Task<NotificationEntity> GetNotifications(UserAccountEntity userAccountEntity)
        {
            var notificationManager = new NotificationManager();
            return await notificationManager.GetNotifications(userAccountEntity.GetUserEntity().OnlineId, userAccountEntity);
        }
    }
}