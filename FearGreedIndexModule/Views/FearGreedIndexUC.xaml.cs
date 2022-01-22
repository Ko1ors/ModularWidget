using FearGreedIndexModule.ViewModels;
using System.Windows.Controls;

namespace FearGreedIndexModule.Views
{
    /// <summary>
    /// Interaction logic for FearGreedIndexUC.xaml
    /// </summary>
    public partial class FearGreedIndexUC : UserControl
    {
        public FearGreedIndexViewModel ViewModel { get; set; }

        public int Test { get; set; }

        public FearGreedIndexUC()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            ViewModel = new FearGreedIndexViewModel();
            await ViewModel.Start();
            DataContext = ViewModel;
        }
    }
}
