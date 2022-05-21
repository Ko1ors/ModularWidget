using ETCModule.ViewModels;
using System.Windows.Controls;

namespace ETCModule.Views
{
    /// <summary>
    /// Логика взаимодействия для EtcPriceUC.xaml
    /// </summary>
    public partial class EtcPriceUC : UserControl
    {
        public EtcPriceUC(EtcPriceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
