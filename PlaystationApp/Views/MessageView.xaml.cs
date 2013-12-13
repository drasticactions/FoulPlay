using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace PlaystationApp.Views
{
    public partial class MessageView : PhoneApplicationPage
    {
        public MessageView()
        {
            InitializeComponent();
        }

        private MessageEntity _messageEntity;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var messagerManager = new MessageManager();
            _messageEntity = await messagerManager.GetGroupConversation(App.SelectedMessageGroupId, App.UserAccountEntity);
            MessageList.DataContext = _messageEntity;
        }

        private void SendMessageButton_OnTap(object sender, GestureEventArgs e)
        {
            App.SelectedMessageEventEntity = _messageEntity;
            NavigationService.Navigate(new Uri("/Views/MessagePostView.xaml", UriKind.Relative));
        }
    }
}