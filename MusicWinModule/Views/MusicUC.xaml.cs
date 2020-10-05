using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
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
        private GlobalSystemMediaTransportControlsSession session;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MusicUC), new FrameworkPropertyMetadata("Title"));
        public static readonly DependencyProperty ArtistProperty = DependencyProperty.Register("Artist", typeof(string), typeof(MusicUC), new FrameworkPropertyMetadata("Artist"));
        public static readonly DependencyProperty ButtonLogoProperty = DependencyProperty.Register("ButtonLogo", typeof(string), typeof(MusicUC), new FrameworkPropertyMetadata("ButtonLogo"));
        private string Title { 
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
            FontAwesome_MSBuildXamlFix();
            InitializeComponent();
            sessionManager = GetGlobalSystemMediaTransportControlsSessionManager();
            if (sessionManager == null)
                throw new Exception(); 
            session = sessionManager.GetCurrentSession();
            if(session != null)
                session.PlaybackInfoChanged += Session_PlaybackInfoChanged;
            else
                sessionManager.CurrentSessionChanged += SessionManager_CurrentSessionChanged;
            TryUpdateOnStart();
        }

        private void TryUpdateOnStart()
        {
            if (session != null)
                TryUpdate(session, 10, 50);
            else
                SetDefault();
        }

        private void SessionManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            var currentSession = sender.GetCurrentSession();
            if (currentSession != null && session == null)
            {
                sessionManager.CurrentSessionChanged -= SessionManager_CurrentSessionChanged;
                session = currentSession;
                session.PlaybackInfoChanged += Session_PlaybackInfoChanged;
                TryUpdate(session, 10, 50);
            }
            else
                SetDefault();
        }

        private void TryUpdate(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {          
            TryUpdateTitleAndArtist(session, tries, timeBetween);
            TryUpdateThumbnail(session, tries, timeBetween);        
        }


        private void Session_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            UpdateLogoButton(sender);
            if (session != null && sender != null)
            {
                session = sender;
                TryUpdate(session, 10, 50);
            }     
        }

        private bool SessionsEquals(GlobalSystemMediaTransportControlsSession s1, GlobalSystemMediaTransportControlsSession s2)
        {
            if (s1 == null && s2 == null)
                return true;
            if (s1 == null || s2 == null)
                return false;
            if (s1.SourceAppUserModelId != s2.SourceAppUserModelId)
                return false;
            var mp1 = s1.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            var mp2 = s2.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            return MediaPropertiesEquals(mp1, mp2);
        }

        private bool MediaPropertiesEquals(GlobalSystemMediaTransportControlsSessionMediaProperties mp1, GlobalSystemMediaTransportControlsSessionMediaProperties mp2)
        {
            if (mp1 == null && mp2 == null)
                return true;
            if (mp1 == null || mp2 == null)
                return false;
            bool t = mp1.AlbumArtist == mp2.AlbumArtist && mp1.AlbumTitle == mp2.AlbumTitle && mp1.AlbumTrackCount == mp2.AlbumTrackCount
                && mp1.Artist == mp2.Artist && mp1.Subtitle == mp2.Subtitle && mp1.Title == mp2.Title && mp1.TrackNumber == mp2.TrackNumber;
            Console.WriteLine(t);
            Console.WriteLine(mp1.Title);
            Console.WriteLine(mp2.Title);
            return t;
        }

        private void UpdateLogoButton(GlobalSystemMediaTransportControlsSession currentSession)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                if (currentSession.GetPlaybackInfo().Controls.IsPauseEnabled)
                    ButtonLogo = "Pause";
                else
                    ButtonLogo = "Play";
            }, null);
        }

        private void TryUpdateThumbnail(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            while(tries > 0)
            {
                try
                {
                    var mediaProperties = session.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
                    if (mediaProperties.Thumbnail == null)
                        throw new Exception();
                    var ras =  mediaProperties.Thumbnail.OpenReadAsync();
                    ras.AsTask().Wait();
                    using (var stream = ras.GetResults().AsStream())
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
                        {
                            thumbnail.ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }, null).Wait();
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

        private void TryUpdateTitleAndArtist(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            while (tries > 0)
            {
                try
                {
                    var mediaProperties = session.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
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
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                Title = null;
                Artist = null;
                thumbnail.ImageSource = null;
            },null);
        }

        private GlobalSystemMediaTransportControlsSessionManager GetGlobalSystemMediaTransportControlsSessionManager()
        {
            return GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
        }

        private static void FontAwesome_MSBuildXamlFix()
        {
            /*
             * WORKAROUND
             * we need this method so that FontAwesome.WPF.dll gets copied as part of the build process
             * 
             */

            var type = typeof(FontAwesome.WPF.FontAwesome);
            Console.WriteLine(type.FullName);
        }
    }
}
