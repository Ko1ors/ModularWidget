using Microsoft.Extensions.Logging;
using MusicWinModule.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicWinModule.Views
{
    /// <summary>
    /// Логика взаимодействия для MusicUC.xaml
    /// </summary>
    public partial class MusicUC : UserControl
    {
        public MusicUC(MediaPlayerViewModel viewModel, ILogger<MusicUC> logger)
        {
            InitializeComponent();
            DataContext = viewModel;
            Task.Run(async () => await viewModel.StartAsync()).ContinueWith((t) =>
            {
                if (t.IsFaulted)
                    logger.LogError(t.Exception, "Media Player Module Error.");
            });
        }
    }
}
