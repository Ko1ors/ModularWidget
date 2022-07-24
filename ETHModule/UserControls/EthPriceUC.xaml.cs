using ETHModule.ViewModels;
using System.Windows.Controls;

namespace ETHModule.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EthPriceUC.xaml
    /// </summary>
    public partial class EthPriceUC : UserControl
    {
        public EthPriceUC(EthPriceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

    }
}
