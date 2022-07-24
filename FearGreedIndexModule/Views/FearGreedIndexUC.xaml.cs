using FearGreedIndexModule.ViewModels;
using System.Windows.Controls;

namespace FearGreedIndexModule.Views
{
    /// <summary>
    /// Interaction logic for FearGreedIndexUC.xaml
    /// </summary>
    public partial class FearGreedIndexUC : UserControl
    {

        public FearGreedIndexUC(FearGreedIndexViewModel viewModel)
        {
            InitializeComponent();
            Init(viewModel);
        }

        private async void Init(FearGreedIndexViewModel viewModel)
        {
            await viewModel.StartAsync();
            DataContext = viewModel;
        }
    }
}
