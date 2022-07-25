using ModularWidget.Commands;
using ModularWidget.Common.ViewModels;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Windows.Media.Control;

namespace MusicWinModule.ViewModels
{
    public class MediaPlayerViewModel : ViewModelBase
    {
        private GlobalSystemMediaTransportControlsSessionManager _sessionManager;
        private GlobalSystemMediaTransportControlsSession _currentSession;
        private long _sessionUpdateTime;

        private string _title;
        private string _artist;
        private string _ButtonLogo;
        private Visibility _moduleVisibility;
        private BitmapFrame _thumbnailSource;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        public string ButtonLogo
        {
            get { return _ButtonLogo; }
            set
            {
                _ButtonLogo = value;
                OnPropertyChanged(nameof(ButtonLogo));
            }
        }

        public Visibility ModuleVisibility
        {
            get { return _moduleVisibility; }
            set
            {
                _moduleVisibility = value;
                OnPropertyChanged(nameof(ModuleVisibility));
            }
        }

        public BitmapFrame ThumbnailSource
        {
            get { return _thumbnailSource; }
            set
            {
                _thumbnailSource = value;
                OnPropertyChanged(nameof(ThumbnailSource));
            }
        }

        public ICommand PlayToggleCommand { get; private set; }

        public ICommand StepForwardCommand { get; private set; }

        public ICommand StepBackwardCommand { get; private set; }

        public MediaPlayerViewModel()
        {
            PlayToggleCommand = new RelayCommand(async (obj) => await PlayToggleAsync());
            StepForwardCommand = new RelayCommand(async (obj) => await StepForwardAsync());
            StepBackwardCommand = new RelayCommand(async (obj) => await StepBackwardAsync());
        }

        public bool CanStart()
        {
            return OperatingSystem.IsWindows() && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041, 0);
        }

        private async Task PlayToggleAsync()
        {
            if (_currentSession is null)
                return;

            if (_sessionManager.GetCurrentSession().GetPlaybackInfo().Controls.IsPauseEnabled)
                await _currentSession.TryPauseAsync();
            else
                await _currentSession.TryPlayAsync();
        }

        private async Task StepBackwardAsync()
        {
            if (_currentSession != null)
                await _currentSession.TrySkipPreviousAsync();
        }

        private async Task StepForwardAsync()
        {
            if (_currentSession != null)
                await _currentSession.TrySkipNextAsync();
        }

        public async Task StartAsync()
        {
            if (!CanStart())
                throw new InvalidOperationException("This operation is not supported on this platform.");

            _sessionManager = await GetGlobalSystemMediaTransportControlsSessionManagerAsync();
            if (_sessionManager == null)
                throw new Exception();
            _sessionManager.CurrentSessionChanged += SessionManagerCurrentSessionChanged;
            TryUpdateOnStart();
        }

        private void TryUpdateOnStart()
        {
            var session = _sessionManager.GetCurrentSession();
            if (session != null)
                TryUpdate(session, 10, 50);
            else
                SetDefault();
        }

        private async Task<GlobalSystemMediaTransportControlsSessionManager> GetGlobalSystemMediaTransportControlsSessionManagerAsync()
        {
            return await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        }

        private void SessionManagerCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            var session = sender.GetCurrentSession();
            if (session != null)
                TryUpdate(session, 10, 50);
            else
                SetDefault();
        }

        private void TryUpdate(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            UpdatePlaybackInfo(session);
            TryUpdateTitleAndArtist(session, 5, 50);
            TryUpdateThumbnail(session, 10, 50);
            ModuleVisibility = Visibility.Visible;
        }

        private void ForceUpdateSession()
        {
            /*
             * WORKAROUND 
             * 
             * 
             */
            Thread.Sleep(500);
            Console.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds() - _sessionUpdateTime);
            if (_sessionManager.GetCurrentSession() == null && DateTimeOffset.Now.ToUnixTimeMilliseconds() - _sessionUpdateTime >= 500)
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

        private void UpdatePlaybackInfo(GlobalSystemMediaTransportControlsSession session)
        {
            UpdateLogoButton(session);
            if (_currentSession != null)
                _currentSession.PlaybackInfoChanged -= Session_PlaybackInfoChanged;
            _currentSession = session;
            _currentSession.PlaybackInfoChanged += Session_PlaybackInfoChanged;
        }

        private async void Session_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            Console.WriteLine(args.ToString());
            if (_currentSession != null && await SessionsEqualsAsync(sender, _currentSession))
            {
                UpdateLogoButton(_currentSession);
                ModuleVisibility = Visibility.Visible;
                TryUpdateTitleAndArtist(_currentSession, 5, 50);
                ForceUpdateThumbnail(_currentSession);
                _sessionUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Task.Run(() => ForceUpdateSession());
            }
        }

        private void UpdateLogoButton(GlobalSystemMediaTransportControlsSession session)
        {
            if (session is null)
                return;

            if (session.GetPlaybackInfo().Controls.IsPauseEnabled)
                ButtonLogo = "Pause";
            else
                ButtonLogo = "Play";
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
                        ThumbnailSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
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

                    Title = mediaProperties.Title;
                    Artist = mediaProperties.Artist;
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
            ModuleVisibility = Visibility.Collapsed;
            Title = null;
            Artist = null;
            ThumbnailSource = null;
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

        private static bool MediaPropertiesEquals(GlobalSystemMediaTransportControlsSessionMediaProperties mp1, GlobalSystemMediaTransportControlsSessionMediaProperties mp2)
        {
            if (mp1 == null && mp2 == null)
                return true;
            if (mp1 == null || mp2 == null)
                return false;
            return mp1.AlbumArtist == mp2.AlbumArtist && mp1.AlbumTitle == mp2.AlbumTitle && mp1.AlbumTrackCount == mp2.AlbumTrackCount
                && mp1.Artist == mp2.Artist && mp1.Subtitle == mp2.Subtitle && mp1.Title == mp2.Title && mp1.TrackNumber == mp2.TrackNumber;
        }
    }
}
