using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EthWidget
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
            AppSettings.Load();
            Manager.Start();
        }

        private void UpdateInformation()
        {    
            this.Dispatcher.Invoke(() =>
            {
                labelEthPrice.Content = $"${Manager.lastEthPrice.Result.Ethusd} ❙ {Manager.lastEthPrice.Result.Ethbtc} BTC";
                labelGasLow.Content = $"{Manager.lastGasPrice.Result.SafeGasPrice} gwei";
                labelGasAvg.Content = $"{Manager.lastGasPrice.Result.ProposeGasPrice} gwei";
                labelGasHigh.Content = $"{Manager.lastGasPrice.Result.FastGasPrice} gwei";
                labelBlockReward.Content = $"{Manager.lastAvgBlockReward} ETH";
                labelWalletBalance.Content = $"{Manager.lastWalletBalance} ETH ❙ ${Math.Round(Double.Parse(Manager.lastEthPrice.Result.Ethusd.Replace('.', ',')) * Manager.lastWalletBalance,2).ToString().Replace(',', '.')}";
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
    }
}
