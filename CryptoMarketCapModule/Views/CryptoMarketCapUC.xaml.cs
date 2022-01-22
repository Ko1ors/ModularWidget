using CryptoMarketCapModule.Services;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace CryptoMarketCapModule.Views
{
    /// <summary>
    /// Interaction logic for CryptoMarketCapUC.xaml
    /// </summary>
    public partial class CryptoMarketCapUC : UserControl
    {
        private readonly IMarketCapService service;
        private readonly int timeInterval = 1;
        private readonly Timer timer;

        public static readonly DependencyProperty MarketCapProperty = DependencyProperty.Register("MarketCap", typeof(long), typeof(CryptoMarketCapUC));

        private long MarketCap
        {
            get
            {
                return (long)GetValue(MarketCapProperty);
            }
            set
            {
                SetValue(MarketCapProperty, value);
            }
        }


        public CryptoMarketCapUC()
        {
            InitializeComponent();
            DataContext = this;
            service = new CoinGeckoMarketCapService();
            UpdateMarketCap();

            timer = new Timer(timeInterval * 60 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateMarketCap();
        }

        private async void UpdateMarketCap()
        {
            var cap = await service?.GetMarketCap();
            if (Dispatcher.CheckAccess())
                MarketCap = cap;
            else
                Dispatcher.Invoke(() =>
                {
                    MarketCap = cap;
                });
        }
    }
}
