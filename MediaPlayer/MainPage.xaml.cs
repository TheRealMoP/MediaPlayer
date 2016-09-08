using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.ApplicationModel;
using Windows.System.Display;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture _mediaCapture;
        private MediaElement _audioMediaElement;
        DisplayRequest _displayRequest;
        bool _isPreviewing;

        public MainPage()
        {
            this.InitializeComponent();

            Application.Current.Suspending += Application_Suspending;
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }
        
        private async Task PlayAudioFile()
        {
            var file = (playlistAudio.SelectedItem as AudioFile)?.StorageFile;
            if (file != null)
            {
                if (_audioMediaElement == null)
                {
                    _audioMediaElement = new MediaElement()
                    {
                        AudioCategory = AudioCategory.Media
                    };
                }

                var stream = await file.OpenAsync(FileAccessMode.Read);
                _audioMediaElement.SetSource(stream, "");
            }
        }

        private async Task<ICollection<AudioFile>> ConvertStorageToAudioFile(IEnumerable<StorageFile> storageFiles)
        {
            
            var audioList = new List<AudioFile>();
            foreach (var file in storageFiles)
            {
                var musicProps = await file.Properties.GetMusicPropertiesAsync();

                audioList.Add(new AudioFile()
                {
                    Title = musicProps.Title == "" ? file.DisplayName : musicProps.Title,
                    Artist = musicProps.Artist,
                    Duration = $"{musicProps.Duration.Minutes:00}:{musicProps.Duration.Seconds:00}",
                    StorageFile = file
                });
            }

            return audioList;
        }

        private async Task SelectAudioFilesAsync()
        {
            var selection = await Common.OpenFileMulti(PickerLocationId.MusicLibrary, FileExtensions.Audio);
            
            if (selection != null && selection.Any())
            {
                playlistAudio.ItemsSource = await ConvertStorageToAudioFile(selection);

                buttonPlayAudio.Visibility = Visibility.Visible;
                buttonStopAudio.Visibility = Visibility.Visible;
            }
        }

        private async Task CleanupCameraAsync()
        {
            if (_mediaCapture == null)
                return;

            await _mediaCapture.StopPreviewAsync();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                webcamPreview.Source = null;
                _displayRequest?.RequestRelease();
            });

            _mediaCapture.Dispose();
            _mediaCapture = null;

            buttonLaunchWebcam.Content = "Launch webcam";
            _isPreviewing = false;
        }

        private async Task StartWebcamAsync()
        {
            try
            {
                _displayRequest = new DisplayRequest();
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();

                webcamPreview.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
                buttonLaunchWebcam.Content = "Stop webcam";
                _isPreviewing = true;

                _displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
            }
        }

        private async void buttonSelectAudioFiles_Click(object sender, RoutedEventArgs e)
        {
            await SelectAudioFilesAsync();
        }

        private async void buttonSelectVideoFile_Click(object sender, RoutedEventArgs e)
        {
            var selection = await Common.OpenFile(PickerLocationId.VideosLibrary, FileExtensions.Video);
        }

        private async void buttonSelectImageFile_Click(object sender, RoutedEventArgs e)
        {
            var selection =await  Common.OpenFile(PickerLocationId.MusicLibrary, FileExtensions.Image);
            if (selection != null)
            {
                IRandomAccessStream fileStream = await selection.OpenAsync(FileAccessMode.Read);
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(fileStream);
                
                image.Stretch = Stretch.Fill;
                image.Source = bmp;
            }
        }

        private async void buttonSelectImageFolder_Click(object sender, RoutedEventArgs e)
        {
            var selection = await Common.OpenFolder(PickerLocationId.PicturesLibrary, FileExtensions.Image);
            if (selection != null)
            {
                StorageItemThumbnail thumbs = await selection.GetThumbnailAsync(ThumbnailMode.PicturesView);
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(thumbs);
                image.Source = bmp;
            }
        }

        private async void buttonLaunchWebcam_Click(object sender, RoutedEventArgs e)
        {
            if (_isPreviewing && _mediaCapture != null)
            {
                await CleanupCameraAsync();
            }
            else
            {
                await StartWebcamAsync();
            }
        }

        private async void buttonCaptureFoto_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
        
        private async void buttonPlayAudioFile_Click(object sender, RoutedEventArgs e)
        {
            if (_audioMediaElement == null)
                return;

            _audioMediaElement.Play();
            if (_audioMediaElement.CurrentState == MediaElementState.Playing)
            {
                _audioMediaElement.Pause();
                buttonPlayAudio.Content = "Play";
            }
            else
            {
                _audioMediaElement.Play();
                buttonPlayAudio.Content = "Pause";
            }
        }

        private void buttonStopAudioFile_Click(object sender, RoutedEventArgs e)
        {
            _audioMediaElement?.Stop();
            buttonPlayAudio.Content = "Play";
        }

        private async void playlistAudio_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await PlayAudioFile();
            buttonPlayAudio.Content = "Pause";
        }
    }
}
