using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PlaystationApp.UserControls
{
    public partial class AcceptFriendUserControl : UserControl
    {
        public AcceptFriendUserControl()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(App.SelectedFriendRequestMessage)) return;
            FriendRequestScrollViewer.Visibility = Visibility.Visible;
            FriendRequestMessageTextBlock.Text = App.SelectedFriendRequestMessage;
        }
    }
}
