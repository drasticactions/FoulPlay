using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace PlaystationApp.Views
{
    public partial class MessageView : PhoneApplicationPage
    {
        public MessageView()
        {
            InitializeComponent();
        }

        private void SendMessageButton_OnTap(object sender, GestureEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}