using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Coding4Fun.Toolkit.Audio.Helpers;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using PlaystationApp.Core.Entity;
using PlaystationApp.Core.Manager;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using PlaystationApp.Resources;
using PlaystationApp.UserControls;
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
            LoadingProgressBar.Visibility = Visibility.Visible;
            var messagerManager = new MessageManager();
            _messageEntity = await messagerManager.GetGroupConversation(App.SelectedMessageGroupId, App.UserAccountEntity);
            MessageList.DataContext = _messageEntity;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void SendMessageButton_OnTap(object sender, GestureEventArgs e)
        {
            App.SelectedMessageEventEntity = _messageEntity;
            NavigationService.Navigate(new Uri("/Views/MessagePostView.xaml", UriKind.Relative));
        }

        private async void MessageList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (MessageEntity.Message)MessageList.SelectedItem;
            if (item == null) return;
            var messageManager = new MessageManager();
            if (item.ContentKeys.HasImage)
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                App.SelectedMessage = item;
                var imageBytes = await
                    messageManager.GetMessageContent(_messageEntity.MessageGroupEntity.MessageGroupId, item,
                        App.UserAccountEntity);
                App.SelectedMessageImage = DecodeImage(imageBytes);
                var messagePrompt = new MessagePrompt { Title = AppResources.Image, Body = new MessageImageUserControl() };
                messagePrompt.Show();
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
            else if (item.ContentKeys.HasAudio)
            {
            // TODO: Add audio support
            }
            MessageList.SelectedItem = null;
        }

        public BitmapImage DecodeImage(Stream array)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(array);
            return bitmapImage;
        }    
    }
}