using CoinMarketCapPortfolioModule.ViewModels;
using System.Windows.Controls;

namespace CoinMarketCapPortfolioModule.Views
{
    /// <summary>
    /// Interaction logic for CoinMarketCapPortfolioUC.xaml
    /// </summary>
    public partial class CoinMarketCapPortfolioUC : UserControl
    {
        public CoinMarketCapPortfolioUC(CoinMarketCapPortfolioViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _ = viewModel.StartAsync();
        }
    }
}
