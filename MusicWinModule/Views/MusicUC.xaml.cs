using System;
using System.IO;
using System.Threading;
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
        public MusicUC()
        {
            FontAwesome_MSBuildXamlFix();
            InitializeComponent();
            sessionManager = GetGlobalSystemMediaTransportControlsSessionManager();
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
        }

        private void SessionManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            var session = sender.GetCurrentSession();
            if (session != null)  
                TryUpdate(session, 10, 50);
        }

        private void TryUpdate(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
            TryUpdateTitleAndArtist(session, 5, 50);
            TryUpdateThumbnail(session, 10, 50);        
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
                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                        {
                            thumbnail.ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }, null).Wait();
                        ras.Close();
                        stream.Close();
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

                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                        {
                            title.Text = mediaProperties.Title;
                            artist.Text = mediaProperties.Artist;
                        }, null).Wait();
                        break;
                }
                catch
                {
                    Thread.Sleep(timeBetween);
                    tries--;
                }
            }
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
