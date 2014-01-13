//#define DEBUG_AGENT
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.ScheduledTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        protected async override void OnInvoke(Microsoft.Phone.Scheduler.ScheduledTask task)
        {
            var userAccountEntity = new UserAccountEntity();
            var authManager = new AuthenticationManager();
            bool loginTest = await authManager.RefreshAccessToken(userAccountEntity);
            if (loginTest)
            {
                UserAccountEntity.User user = await authManager.GetUserEntity(userAccountEntity);
                if (user == null) return;
                userAccountEntity.SetUserEntity(user);
                NotificationEntity notificationEntity = await GetNotifications(userAccountEntity);
                if (notificationEntity == null) return;
                var notificationList = notificationEntity.Notifications.Where(o => o.SeenFlag == false);
                NotificationEntity.Notification firstNotification = notificationList.FirstOrDefault();
                ShellTile appTile = ShellTile.ActiveTiles.First();
                if (firstNotification != null)
                {
                    var toastMessage = firstNotification.Message;
                    var toast = new ShellToast { Title = "FoulPlay", Content = toastMessage };
                    toast.Show();
                    if (appTile != null)
                    {
                        var tileData = new FlipTileData
                        {
                            Title = "FoulPlay",
                            BackTitle = "FoulPlay",
                            BackContent = firstNotification.Message,
                            WideBackContent = firstNotification.Message,
                            Count = notificationList.Count()
                        };
                        appTile.Update(tileData);
                    }
                    await NotificationManager.ClearNotification(firstNotification, userAccountEntity);
                }
            }
#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif
            NotifyComplete();
        }

        private async Task<NotificationEntity> GetNotifications(UserAccountEntity userAccountEntity)
        {
            var notificationManager = new NotificationManager();
            return await notificationManager.GetNotifications(userAccountEntity.GetUserEntity().OnlineId, userAccountEntity);
        }
    }
}