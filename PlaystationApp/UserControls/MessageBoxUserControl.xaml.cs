using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using PlaystationApp.Core.Manager;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlaystationApp.UserControls
{
    public partial class MessageBoxUserControl
    {
        private readonly CameraCaptureTask _cameraCaptureTask;
        public MessageBoxUserControl()
        {
            InitializeComponent();
            _cameraCaptureTask = new CameraCaptureTask();
            _cameraCaptureTask.Completed += cameraCaptureTask_Completed;
        }

        private async void StatusPostButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            string messageId = App.SelectedMessageGroupId;
            var messageManager = new MessageManager();
            StatusPostButton.IsEnabled = false;
            ImagePicker.IsEnabled = false;
            CameraAccess.IsEnabled = false;
            bool result;
            if (ImagePickerImage.Source != null)
            {
                var bmp = new WriteableBitmap((BitmapSource) ImagePickerImage.Source);
                byte[] byteArray;
                using (var stream = new MemoryStream())
                {
                    bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 50);
                    byteArray = stream.ToArray();
                }
                result = await
                    messageManager.CreatePostWithMedia(messageId, StatusUpdateBox.Text, "", byteArray,
                        App.UserAccountEntity);
            }
            else
            {
                result = await messageManager.CreatePost(messageId, StatusUpdateBox.Text, App.UserAccountEntity);
            }

            if(result != true)
            {
                MessageBox.Show("An error has occurred.");
                StatusPostButton.IsEnabled = true;
                ImagePicker.IsEnabled = true;
                CameraAccess.IsEnabled = true;
            }
            else
            {
                var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (rootFrame != null)
                    rootFrame.GoBack();
            }
        }

        private void ImagePicker_Click(object sender, RoutedEventArgs e)
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;
            BitmapImage bitmap = new BitmapImage { CreateOptions = BitmapCreateOptions.None };
            bitmap.SetSource(e.ChosenPhoto);
            
            ImagePickerImage.Visibility = Visibility.Visible;
            ImagePickerImage.Source = bitmap;
        }

        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;
            var bmp = new BitmapImage();
            bmp.SetSource(e.ChosenPhoto);
            ImagePickerImage.Visibility = Visibility.Visible;
            ImagePickerImage.Source = bmp;
        }


        private void CameraAccess_Click(object sender, RoutedEventArgs e)
        {
            _cameraCaptureTask.Show();
        }
    }
}