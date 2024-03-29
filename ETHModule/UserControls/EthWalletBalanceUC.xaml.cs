﻿using ETHModule.ViewModels;
using System.Windows.Controls;

namespace ETHModule.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EthWalletBalanceUC.xaml
    /// </summary>
    public partial class EthWalletBalanceUC : UserControl
    {
        public EthWalletBalanceUC(EthWalletBalanceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
