using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Coding4Fun.Toolkit.Audio.Helpers;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            var refreshButton =
                new ApplicationBarIconButton(new Uri("/Assets/AppBar/sync.png", UriKind.Relative))
                {
                    Text = AppResources.Refresh
                };
            refreshButton.Click += RefreshButton_Click;

            ApplicationBar.Buttons.Add(refreshButton);
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await RefreshGroupMessages();
        }

        private MessageEntity _messageEntity;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await RefreshGroupMessages();
        }

        private async Task<bool> RefreshGroupMessages()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var messagerManager = new MessageManager();
            _messageEntity = await messagerManager.GetGroupConversation(App.SelectedMessageGroupId, App.UserAccountEntity);
            if (_messageEntity == null)
            {
                MessageBox.Show(AppResources.GenericError);
                var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (rootFrame != null)
                    rootFrame.GoBack();
                return false;
            }
            MessageList.DataContext = _messageEntity;
            await messagerManager.ClearMessages(_messageEntity, App.UserAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
            SendMessageButton.IsEnabled = true;
            return true;
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
            try
            {
                bitmapImage.SetSource(array);
                return bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }    
    }
}