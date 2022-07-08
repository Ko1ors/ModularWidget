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

        public FearGreedIndexUC(FearGreedIndexViewModel viewModel)
        {
            InitializeComponent();
            Init(viewModel);
        }

        private async void Init(FearGreedIndexViewModel viewModel)
        {
            await viewModel.Start();
            DataContext = viewModel;
        }
    }
}
