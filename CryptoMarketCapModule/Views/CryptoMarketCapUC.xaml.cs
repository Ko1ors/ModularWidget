using CryptoMarketCapModule.Services;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IMarketCapService _marketCapService;
        private readonly ILogger<CryptoMarketCapUC> _logger;
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


        public CryptoMarketCapUC(IMarketCapService marketCapService, ILogger<CryptoMarketCapUC> logger)
        {
            _marketCapService = marketCapService;
            _logger = logger;
            InitializeComponent();
            DataContext = this;
            UpdateMarketCap();
            timer = new Timer(timeInterval * 60 * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _logger.LogInformation("Timer_Elapsed.");
            UpdateMarketCap();
        }

        private async void UpdateMarketCap()
        {
            try
            {
                _logger.LogInformation("Updating market cap");
                var cap = await _marketCapService?.GetMarketCap();
                if (Dispatcher.CheckAccess())
                    MarketCap = cap;
                else
                    Dispatcher.Invoke(() =>
                    {
                        MarketCap = cap;
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating market cap");
            }
        }
    }
}
