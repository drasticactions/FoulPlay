using System;
using System.Windows;
using Microsoft.Phone.Controls;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace PlaystationApp.Views
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private UserEntity User { get; set; }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            var userManager = new UserManager();
            User = await userManager.GetUser(SearchBox.Text, App.UserAccountEntity);
            NoResultsFoundBlock.Visibility = string.IsNullOrEmpty(User.OnlineId)
                ? Visibility.Visible
                : Visibility.Collapsed;
            UserSearchResultGrid.DataContext = User;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void UserSearchResultGrid_OnTap(object sender, GestureEventArgs e)
        {
            App.SelectedUser = User;
            NavigationService.Navigate(new Uri("/Views/UserPage.xaml", UriKind.Relative));
        }
    }
}