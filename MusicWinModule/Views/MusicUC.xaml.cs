using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;

namespace MusicWinModule.Views
{
    /// <summary>
    /// Логика взаимодействия для MusicUC.xaml
    /// </summary>
    public partial class MusicUC : UserControl
    {
        private GlobalSystemMediaTransportControlsSessionManager sessionManager;
        private GlobalSystemMediaTransportControlsSession currentSession;
        private long sessionUpdateTime;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MusicUC), new FrameworkPropertyMetadata("Title"));
        public static readonly DependencyProperty ArtistProperty = DependencyProperty.Register("Artist", typeof(string), typeof(MusicUC), new FrameworkPropertyMetadata("Artist"));
        public static readonly DependencyProperty ButtonLogoProperty = DependencyProperty.Register("ButtonLogo", typeof(string), typeof(MusicUC), new FrameworkPropertyMetadata("ButtonLogo"));
        private string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        private string Artist
        {
            get
            {
                return (string)this.GetValue(ArtistProperty);
            }
            set
            {
                this.SetValue(ArtistProperty, value);
            }
        }


        private string ButtonLogo
        {
            get
            {
                return (string)this.GetValue(ButtonLogoProperty);
            }
            set
            {
                this.SetValue(ButtonLogoProperty, value);
            }
        }

        public MusicUC()
        {
            if (OperatingSystem.IsWindows() && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041, 0))
            {
                InitializeComponent();
                Init();
            }
        }

        public async void Init()
        {
            sessionManager = await GetGlobalSystemMediaTransportControlsSessionManagerAsync();
            if (sessionManager == null)
                throw new Exception();
            sessionManager.CurrentSessionChanged += SessionManager_CurrentSessionChanged;
            TryUpdateOnStart();
        }


        private void TryUpdateOnStart()
        {
            var session = sessionManager.GetCurrentSession();
            if (session != null)
                TryUpdate(session, 10, 50);
            else
                SetDefault();
        }

        private void SessionManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            var session = sender.GetCurrentSession();
            if (session != null)
                TryUpdate(session, 10, 50);
            else
                SetDefault();
        }

        private void TryUpdate(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            SetVisibility(Visibility.Visible);
            UpdatePlaybackInfo(session);
            TryUpdateTitleAndArtist(session, 5, 50);
            TryUpdateThumbnail(session, 10, 50);
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                Visibility = Visibility.Visible;
            }, null);
        }

        private void UpdatePlaybackInfo(GlobalSystemMediaTransportControlsSession session)
        {
            UpdateLogoButton(session);
            if (currentSession != null)
                currentSession.PlaybackInfoChanged -= Session_PlaybackInfoChanged;
            currentSession = session;
            currentSession.PlaybackInfoChanged += Session_PlaybackInfoChanged;
        }

        private async void Session_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            Console.WriteLine(args.ToString());
            if (currentSession != null && await SessionsEqualsAsync(sender, currentSession))
            {
                UpdateLogoButton(currentSession);
                SetVisibility(Visibility.Visible);
                TryUpdateTitleAndArtist(currentSession, 5, 50);
                ForceUpdateThumbnail(currentSession);
                sessionUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Task.Run(() => ForceUpdateSession());
            }
        }

        private void ForceUpdateSession()
        {
            /*
             * WORKAROUND 
             * 
             * 
             */
            Thread.Sleep(500);
            Console.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds() - sessionUpdateTime);
            if (sessionManager.GetCurrentSession() == null && DateTimeOffset.Now.ToUnixTimeMilliseconds() - sessionUpdateTime >= 500)
                SetDefault();
        }

        private void ForceUpdateThumbnail(GlobalSystemMediaTransportControlsSession sender)
        {
            /*
             * WORKAROUND 
             * 
             * 
             */

            Thread.Sleep(100);
            TryUpdateThumbnail(sender, 10, 50);
        }

        private void UpdateLogoButton(GlobalSystemMediaTransportControlsSession session)
        {
            if (session != null)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
                {
                    if (session.GetPlaybackInfo().Controls.IsPauseEnabled)
                        ButtonLogo = "Pause";
                    else
                        ButtonLogo = "Play";
                }, null);
            }
        }

        private async void TryUpdateThumbnail(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            while (tries > 0)
            {
                try
                {
                    var mediaProperties = await session.TryGetMediaPropertiesAsync();
                    if (mediaProperties.Thumbnail == null)
                        throw new Exception();
                    var ras = mediaProperties.Thumbnail.OpenReadAsync();
                    ras.AsTask().Wait();
                    using (var stream = (await ras).AsStream())
                    {
                        await Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
                        {
                            thumbnail.ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }, null);
                        ras.Close();
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(timeBetween);
                    tries--;
                }
            }
        }

        private async void TryUpdateTitleAndArtist(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            while (tries > 0)
            {
                try
                {
                    var mediaProperties = await session.TryGetMediaPropertiesAsync();
                    if (mediaProperties.Title == null && mediaProperties.Artist == null)
                        throw new Exception();

                    Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
                    {
                        Title = mediaProperties.Title;
                        Artist = mediaProperties.Artist;
                    }, null);
                    break;
                }
                catch
                {
                    Thread.Sleep(timeBetween);
                    tries--;
                }
            }
        }

        private void SetDefault()
        {
            SetVisibility(Visibility.Collapsed);
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                Visibility = Visibility.Collapsed;
                Title = null;
                Artist = null;
                thumbnail.ImageSource = null;

            }, null);
        }

        private void SetVisibility(Visibility visibility)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                Visibility = visibility;
            }, null);
        }

        private async Task<bool> SessionsEqualsAsync(GlobalSystemMediaTransportControlsSession s1, GlobalSystemMediaTransportControlsSession s2)
        {
            if (s1 == null && s2 == null)
                return true;
            if (s1 == null || s2 == null)
                return false;
            if (s1.SourceAppUserModelId != s2.SourceAppUserModelId)
                return false;
            var mp1 = await s1.TryGetMediaPropertiesAsync();
            var mp2 = await s2.TryGetMediaPropertiesAsync();
            return MediaPropertiesEquals(mp1, mp2);
        }

        private bool MediaPropertiesEquals(GlobalSystemMediaTransportControlsSessionMediaProperties mp1, GlobalSystemMediaTransportControlsSessionMediaProperties mp2)
        {
            if (mp1 == null && mp2 == null)
                return true;
            if (mp1 == null || mp2 == null)
                return false;
            return mp1.AlbumArtist == mp2.AlbumArtist && mp1.AlbumTitle == mp2.AlbumTitle && mp1.AlbumTrackCount == mp2.AlbumTrackCount
                && mp1.Artist == mp2.Artist && mp1.Subtitle == mp2.Subtitle && mp1.Title == mp2.Title && mp1.TrackNumber == mp2.TrackNumber;
        }

        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetGlobalSystemMediaTransportControlsSessionManagerAsync()
        {
            return await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        }


        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession != null)
            {
                if (sessionManager.GetCurrentSession().GetPlaybackInfo().Controls.IsPauseEnabled)
                    currentSession.TryPauseAsync();
                else
                    currentSession.TryPlayAsync();
            }
        }

        private void StepBackward_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession != null)
                currentSession.TrySkipPreviousAsync();
        }

        private void StepForward_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession != null)
                currentSession.TrySkipNextAsync();
        }
    }
}
