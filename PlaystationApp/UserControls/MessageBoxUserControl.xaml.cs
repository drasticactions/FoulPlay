using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Coding4Fun.Toolkit.Audio.Helpers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PlaystationApp.Core.Manager;

namespace PlaystationApp.UserControls
{
    public partial class MessageBoxUserControl : UserControl
    {
        private readonly BitmapImage blankImage;
        private readonly CameraCaptureTask cameraCaptureTask;

        private readonly Microphone microphone = Microphone.Default;
            // Object representing the physical microphone on the device

        private readonly BitmapImage microphoneImage;
        private readonly BitmapImage speakerImage;

        private readonly MemoryStream stream = new MemoryStream(); // Stores the audio data for later playback
        private byte[] buffer; // Dynamic buffer to retrieve audio data from the microphone
        private SoundEffectInstance soundInstance; // Used to play back audio
        private bool soundIsPlaying; // Flag to monitor the state of sound playback

        // Status images

        public MessageBoxUserControl()
        {
            InitializeComponent();

            // Timer to simulate the XNA Framework game loop (Microphone is 
            // from the XNA Framework). We also use this timer to monitor the 
            // state of audio playback so we can update the UI appropriately.
            var dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(33);
            dt.Tick += dt_Tick;
            dt.Start();

            // Event handler for getting audio data when the buffer is full
            microphone.BufferReady += microphone_BufferReady;

            blankImage = new BitmapImage(new Uri("/Images/blank.png", UriKind.RelativeOrAbsolute));
            microphoneImage = new BitmapImage(new Uri("/Images/microphone.png", UriKind.RelativeOrAbsolute));
            speakerImage = new BitmapImage(new Uri("/Images/speaker.png", UriKind.RelativeOrAbsolute));

            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += cameraCaptureTask_Completed;
        }

        /// <summary>
        ///     Updates the XNA FrameworkDispatcher and checks to see if a sound is playing.
        ///     If sound has stopped playing, it updates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_Tick(object sender, EventArgs e)
        {
            try
            {
                FrameworkDispatcher.Update();
            }
            catch
            {
            }

            if (soundIsPlaying)
            {
                if (soundInstance.State != SoundState.Playing)
                {
                    // Audio has finished playing
                    soundIsPlaying = false;

                    // Update the UI to reflect that the 
                    // sound has stopped playing
                    //SetButtonStates(true, true, false);
                    //UserHelp.Text = "press play\nor record";
                    //StatusImage.Source = blankImage;
                    PlayButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    RecordButton.IsEnabled = true;
                    MicrophoneImage.Source = blankImage;
                }
            }
        }

        /// <summary>
        ///     The Microphone.BufferReady event handler.
        ///     Gets the audio data from the microphone and stores it in a buffer,
        ///     then writes that buffer to a stream for later playback.
        ///     Any action in this event handler should be quick!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void microphone_BufferReady(object sender, EventArgs e)
        {
            // Retrieve audio data
            microphone.GetData(buffer);

            // Store the audio data in a stream
            stream.Write(buffer, 0, buffer.Length);
        }

        private async void StatusPostButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            var memberNames = App.SelectedMessageEventEntity.MessageGroupEntity.MessageGroupDetail.Members;
            var usernameList = memberNames.Select(member => member.OnlineId).ToList();
            string messageId = string.Format("~{0}", string.Join(",", usernameList.ToArray()));
            var messageManager = new MessageManager();
            StatusPostButton.IsEnabled = false;
            ImagePicker.IsEnabled = false;
            CameraAccess.IsEnabled = false;
            if (ImagePickerImage.Source != null)
            {
                var bmp = new WriteableBitmap((BitmapSource) ImagePickerImage.Source);
                byte[] byteArray;
                using (var stream = new MemoryStream())
                {
                    bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);

                    byteArray = stream.ToArray();
                }
                await
                    messageManager.CreatePostWithMedia(messageId, StatusUpdateBox.Text, "", byteArray,
                        App.UserAccountEntity);
            }
            else if (buffer != null)
            {
                byte[] byteArray;
                using (var stream = new MemoryStream())
                {
                    byteArray = stream.GetWavAsByteArray(microphone.SampleRate);
                }
                await
                    messageManager.CreatePostWithAudio(messageId, StatusUpdateBox.Text, byteArray, App.UserAccountEntity);
            }
            else
            {
                await messageManager.CreatePost(messageId, StatusUpdateBox.Text, App.UserAccountEntity);
            }
            var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
                rootFrame.GoBack();
        }

        private void ImagePicker_Click(object sender, RoutedEventArgs e)
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.ChosenPhoto.Length.ToString());

                //Code to display the photo on the page in an image control named myImage.
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                ImagePickerImage.Visibility = Visibility.Visible;
                ImagePickerImage.Source = bmp;
            }
        }

        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                ImagePickerImage.Visibility = Visibility.Visible;
                ImagePickerImage.Source = bmp;
            }
        }


        private void CameraAccess_Click(object sender, RoutedEventArgs e)
        {
            cameraCaptureTask.Show();
        }

        private void RecordButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Get audio data in 1/2 second chunks
            microphone.BufferDuration = TimeSpan.FromMilliseconds(500);
            RecordButton.IsEnabled = false;
            PlayButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            // Allocate memory to hold the audio data
            buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];

            // Set the stream back to zero in case there is already something in it
            stream.SetLength(0);

            // Start recording
            microphone.Start();
            MicrophoneImage.Visibility = Visibility.Visible;
            MicrophoneImage.Source = microphoneImage;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (microphone.State == MicrophoneState.Started)
            {
                // In RECORD mode, user clicked the 
                // stop button to end recording
                microphone.Stop();
            }
            else if (soundInstance.State == SoundState.Playing)
            {
                // In PLAY mode, user clicked the 
                // stop button to end playing back
                soundInstance.Stop();
            }
            PlayButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
            RecordButton.IsEnabled = true;
            MicrophoneImage.Source = blankImage;
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (stream.Length > 0)
            {
                // Update the UI to reflect that
                // sound is playing
                //SetButtonStates(false, false, true);
                //UserHelp.Text = "play";
                MicrophoneImage.Source = speakerImage;
                RecordButton.IsEnabled = false;
                PlayButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
                // Play the audio in a new thread so the UI can update.
                var soundThread = new Thread(PlaySound);
                soundThread.Start();
            }
        }

        /// <summary>
        ///     Plays the audio using SoundEffectInstance
        ///     so we can monitor the playback status.
        /// </summary>
        private void PlaySound()
        {
            // Play audio using SoundEffectInstance so we can monitor it's State 
            // and update the UI in the dt_Tick handler when it is done playing.
            var sound = new SoundEffect(stream.ToArray(), microphone.SampleRate, AudioChannels.Mono);
            soundInstance = sound.CreateInstance();
            soundIsPlaying = true;
            soundInstance.Play();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusPostButton.IsEnabled = true;
            ImagePicker.IsEnabled = true;
            CameraAccess.IsEnabled = true;
            MicrophoneButton.IsEnabled = true;
            MicrophonePanel.Visibility = Visibility.Collapsed;
            MicrophoneImage.Visibility = Visibility.Collapsed;
            ImagePickerImage.Visibility = Visibility.Visible;
        }

        private void MicrophoneButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusPostButton.IsEnabled = false;
            ImagePicker.IsEnabled = false;
            CameraAccess.IsEnabled = false;
            MicrophoneButton.IsEnabled = false;
            ImagePickerImage.Source = null;
            MicrophonePanel.Visibility = Visibility.Visible;
            ImagePickerImage.Visibility = Visibility.Collapsed;
            MicrophoneImage.Visibility = Visibility.Visible;
        }
    }
}