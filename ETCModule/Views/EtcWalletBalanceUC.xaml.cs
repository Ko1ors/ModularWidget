using ETCModule.ViewModels;
using System.Windows.Controls;

namespace ETCModule.Views
{
    /// <summary>
    /// Логика взаимодействия для EtcWalletBalanceUC.xaml
    /// </summary>
    public partial class EtcWalletBalanceUC : UserControl
    {
        public EtcWalletBalanceUC(EtcWalletBalanceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
