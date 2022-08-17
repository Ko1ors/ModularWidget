using MusicWinModule.ViewModels;
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
        public MusicUC(MediaPlayerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Task.Run(async () => await viewModel.StartAsync());
        }
    }
}
