using ETCModule.Models;
using ETCModule.Settings;
using ModularWidget;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETCModule.Services
{
    public class EtcService : IEtcService
    {
        private readonly AppSettings _appSettings;
        private readonly IEtcClientService _clientService;
        private System.Timers.Timer _timer;
        private string _etcWalletAddress;
        private int maxTries = 5;
        private const double _divisor = 1000000000000000000d;

        public event Updated<EtcPriceResult> EtcPriceUpdated;
        public event Updated<double> WalletBalanceUpdated;
        public event Updated<EtcCompositeResult> EtcUpdated;

        public EtcService(AppSettings settings, IEtcClientService etcClientService)
        {
            _appSettings = settings;
            _clientService = etcClientService;
        }

        public async Task StartAsync()
        {
            await LoadWalletSettingsAsync();
            await UpdateAsync();
            SetTimer();         
        }

        private void SetTimer()
        {
            var time = _appSettings.Get<int>(Constants.Parameters.UpdateTime);
            if (time <= 0)
                time = 5;
            _timer = new System.Timers.Timer(time * 60 * 1000);
            _timer.Elapsed += (s, e) => { _ = UpdateAsync(); };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async Task LoadWalletSettingsAsync()
        {
            _etcWalletAddress = _appSettings.Get<string>(Constants.Parameters.Wallet);
            if (string.Equals(_etcWalletAddress, "random", StringComparison.InvariantCultureIgnoreCase))
                _etcWalletAddress = await GetNonEmptyEtcWalletAsync();
        }

        private async Task<string> GetNonEmptyEtcWalletAsync()
        {
            string wallet = null;
            for (int i = 0; i < maxTries && string.IsNullOrEmpty(wallet); i++)
            {
                
                wallet = (await _clientService.GetRandomWalletsAsync(10))?.EtcWallets.Find(e => !e.Balance.Equals(0) && Convert.ToDouble(e.Balance) / _divisor > 1)?.Address;
            }
            return wallet;
        }

        private async Task UpdateAsync()
        {
            var result = new EtcCompositeResult();
            if (!string.IsNullOrEmpty(_etcWalletAddress))
            {
                result.WalletBalance = await GetEtcWalletBalanceAsync(_etcWalletAddress);
                WalletBalanceUpdated?.Invoke(result.WalletBalance);
            }
            var priceResult = await GetEtcPriceAsync();
            result.Price = priceResult.Price;
            EtcPriceUpdated?.Invoke(priceResult);
            EtcUpdated?.Invoke(result);
        }

        public async Task<EtcPriceResult> GetEtcPriceAsync()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var priceModel = await _clientService.GetPriceAsync();
                if (priceModel.Status == "1" && priceModel.Message == "OK")
                {
                    priceModel.Price.CoinBtc = Math.Round(priceModel.Price.CoinBtc ?? -1, 8);
                    priceModel.Price.CoinUsd = Math.Round(priceModel.Price.CoinUsd ?? -1, 2);
                    return priceModel;
                }
                await Task.Delay(500 * (i + 1));
            }
            return null;
        }

        public async Task<double> GetEtcWalletBalanceAsync(string wallet)
        {
            for (int i = 0; i < maxTries; i++)
            {
                var walletBalanceModel = await _clientService.GetWalletBalanceAsync(_etcWalletAddress);
                if (!walletBalanceModel.Result.Contains("Invalid address hash"))
                {
                    return Math.Round(float.Parse(walletBalanceModel.Result) / _divisor, 4);
                }
                await Task.Delay(500 * (i + 1));
            }
            return -1;
        }
    }
}
