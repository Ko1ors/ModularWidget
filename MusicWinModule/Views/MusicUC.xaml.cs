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
        }

        private void SessionManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            var session = sender.GetCurrentSession();
            if (session != null)  
            {
                TryUpdate(session, 10, 50);
            }
        }

        private void TryUpdate(GlobalSystemMediaTransportControlsSession session, int tries, int timeBetween)
        {
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
                    var stream = ras.GetResults().AsStream();
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate
                    {
                        thumbnail.ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        stream.Close();
                    }, null);   
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

        private async System.Threading.Tasks.Task UpdateThumbnailAsync(GlobalSystemMediaTransportControlsSession session)
        {
            
           //var sessionManager = GetGlobalSystemMediaTransportControlsSessionManager();
            //var gsmtcsm = sessionManager.GetCurrentSession();
            var mediaProperties = session.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            var ras = await mediaProperties.Thumbnail.OpenReadAsync();
            
            var stream = ras.AsStream();
            thumbnail.ImageSource = BitmapFrame.Create(stream,
                                                  BitmapCreateOptions.None,
                                                  BitmapCacheOption.OnLoad);
            stream.Close();
        }

        private async System.Threading.Tasks.Task UpdateThumbnailAsync()
        {
            var gsmtcsm = sessionManager.GetCurrentSession();
            var mediaProperties = gsmtcsm.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            var ras = await mediaProperties.Thumbnail.OpenReadAsync();
            Console.WriteLine(mediaProperties.Thumbnail.ToString());
            var stream = ras.AsStream();
            thumbnail.ImageSource = BitmapFrame.Create(stream,
                                                  BitmapCreateOptions.None,
                                                  BitmapCacheOption.OnLoad);
            stream.Close();
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
