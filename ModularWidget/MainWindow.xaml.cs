using ModularWidget.UserControls;
using System;

using System.Windows;
using System.Windows.Controls;

namespace ModularWidget
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width * 1.1;
            this.Top = SystemParameters.PrimaryScreenHeight * 0.05;

            Manager.Completed += UpdateInformation;
            Manager.RegionRequested += CreateRegion;

            AppSettings.Load();
            if (String.IsNullOrEmpty(AppSettings.ethWallet))
                ethWalletBalanceUC.Visibility = Visibility.Collapsed;
            Manager.Start();
        }

        private void UpdateInformation()
        {
            this.Dispatcher.Invoke(() =>
            {
                ethPriceUC.labelEthPrice.Content = $"${Manager.lastEthPrice.Result.Ethusd} ❙ {Manager.lastEthPrice.Result.Ethbtc} BTC";
                gasTrackerUC.labelGasLow.Content = $"{Manager.lastGasPrice.Result.SafeGasPrice} gwei";
                gasTrackerUC.labelGasAvg.Content = $"{Manager.lastGasPrice.Result.ProposeGasPrice} gwei";
                gasTrackerUC.labelGasHigh.Content = $"{Manager.lastGasPrice.Result.FastGasPrice} gwei";
                blockRewardUC.labelBlockReward.Content = $"{Manager.lastAvgBlockReward} ETH";
                if (!String.IsNullOrEmpty(AppSettings.ethWallet))
                    ethWalletBalanceUC.labelWalletBalance.Content = $"{Manager.lastWalletBalance} ETH ❙ ${Math.Round(Double.Parse(Manager.lastEthPrice.Result.Ethusd.Replace('.', ',')) * Manager.lastWalletBalance, 2).ToString().Replace(',', '.')}";
            });
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.ShowDialog();
        }

        private void CreateRegion(string regName)
        {
            RegionUC reg = new RegionUC();
            reg.RegionName = regName;
            AddRegion(reg);
            Manager.RegionCreate(reg.RegionName);
        }

        private void AddRegion(RegionUC reg)
        {
           stackPanelBlock.Children.Add(reg);
        }
    }
}
