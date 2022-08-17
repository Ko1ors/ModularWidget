using ETHModule.ViewModels;
using System.Windows.Controls;

namespace ETHModule.UserControls
{
    /// <summary>
    /// Логика взаимодействия для BlockRewardUC.xaml
    /// </summary>
    public partial class BlockRewardUC : UserControl
    {
        public BlockRewardUC(BlockRewardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
