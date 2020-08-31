using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace EthWidget
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width * 1.15;
            this.Top = SystemParameters.PrimaryScreenHeight * 0.1;
            textBoxApiKey.Text = AppSettings.ethApiKey;
            textBoxEthWallet.Text = AppSettings.ethWallet;
            textBoxUpdateTime.Text = AppSettings.updateTime.ToString();       
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBoxUpdateTime.Text, out int time);
            if (time == 0)
                time = 5;
            AppSettings.Set(textBoxApiKey.Text,textBoxEthWallet.Text,time);
            AppSettings.Save();
            Close();
        }
    }
}
