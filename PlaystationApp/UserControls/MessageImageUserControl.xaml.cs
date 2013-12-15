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
    public partial class MessageImageUserControl : UserControl
    {
        public MessageImageUserControl()
        {
            InitializeComponent();
            UserMessageImage.Source = App.SelectedMessageImage;
            LayoutRoot.DataContext = App.SelectedMessage;
        }

    }
}
