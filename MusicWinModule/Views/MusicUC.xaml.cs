using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Media.Control;

namespace MusicWinModule.Views
{
    /// <summary>
    /// Логика взаимодействия для MusicUC.xaml
    /// </summary>
    public partial class MusicUC : UserControl
    {
        public MusicUC()
        {
            InitializeComponent();
            UpdateThumbnailAsync();
        }

        private async System.Threading.Tasks.Task UpdateThumbnailAsync()
        {
            var gsmtcsm = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult().GetCurrentSession();
            var mediaProperties = gsmtcsm.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            var ras = await mediaProperties.Thumbnail.OpenReadAsync();
            var stream = ras.AsStream();
            thumbnail.ImageSource = BitmapFrame.Create(stream,
                                                  BitmapCreateOptions.None,
                                                  BitmapCacheOption.OnLoad);
            stream.Close();
        }
    }
}
