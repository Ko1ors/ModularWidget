using ETCModule.Models;
using ETCModule.Settings;
using Microsoft.Extensions.Logging;
using ModularWidget;
using ModularWidget.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ETCModule.Services
{
    public class EtcService : IEtcService
    {
        private readonly SettingsMenu _settingsMenu;
        private readonly IEtcClientService _clientService;
        private readonly ILogger<EtcService> _logger;

        private System.Timers.Timer _timer;
        private string _etcWalletAddress;
        private int maxTries = 5;
        private const double _divisor = 1000000000000000000d;

        public event Updated<EtcPriceResult> EtcPriceUpdated;
        public event Updated<double> WalletBalanceUpdated;
        public event Updated<EtcCompositeResult> EtcUpdated;

        public EtcService(AppSettings settings, IEtcClientService etcClientService, ILogger<EtcService> logger)
        {
            _settingsMenu = settings.GetMenu(Constants.Menu.MenuKey);
            _clientService = etcClientService;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _logger.LogInformation("Starting ETC service.");
            await LoadWalletSettingsAsync();
            await UpdateAsync();
            SetTimer();
        }

        private void SetTimer()
        {
            var time = _settingsMenu.Get<int>(Constants.Parameters.UpdateTime);
            if (time <= 0)
                time = 5;
            _logger.LogInformation($"Setting timer to {time} minutes.");
            _timer = new System.Timers.Timer(time * 60 * 1000);
            _timer.Elapsed += (s, e) => { _ = UpdateAsync(); };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async Task LoadWalletSettingsAsync()
        {
            _logger.LogInformation("Loading wallet settings.");
            _etcWalletAddress = _settingsMenu.Get<string>(Constants.Parameters.Wallet);
            if (string.Equals(_etcWalletAddress, "random", StringComparison.InvariantCultureIgnoreCase))
                _etcWalletAddress = await GetNonEmptyEtcWalletAsync();
            _logger.LogInformation($"Wallet address: {_etcWalletAddress}");
        }

        private async Task<string> GetNonEmptyEtcWalletAsync()
        {
            _logger.LogInformation("Getting non-empty wallet address.");
            string wallet = null;
            for (int i = 0; i < maxTries && string.IsNullOrEmpty(wallet); i++)
            {
                wallet = (await _clientService.GetRandomWalletsAsync(10))?.EtcWallets.Find(e => !e.Balance.Equals(0) && Convert.ToDouble(e.Balance) / _divisor > 1)?.Address;
            }
            _logger.LogInformation($"Non-empty wallet address: {wallet}");
            return wallet;
        }

        private async Task UpdateAsync()
        {
            _logger.LogInformation("Updating ETC prices.");
            var result = new EtcCompositeResult();
            if (!string.IsNullOrEmpty(_etcWalletAddress))
            {
                result.WalletBalance = await GetEtcWalletBalanceAsync(_etcWalletAddress);
                WalletBalanceUpdated?.Invoke(result.WalletBalance);
            }
            var priceResult = await GetEtcPriceAsync();
            result.Price = priceResult.Price;
            _logger.LogInformation($"ETC price updated. Result model: {JsonConvert.SerializeObject(result)}");
            EtcPriceUpdated?.Invoke(priceResult);
            EtcUpdated?.Invoke(result);
        }

        public async Task<EtcPriceResult> GetEtcPriceAsync()
        {
            try
            {
                for (int i = 0; i < maxTries; i++)
                {
                    _logger.LogInformation($"Getting ETC price. Try {i + 1} of {maxTries}.");
                    var priceModel = await _clientService.GetPriceAsync();
                    if (priceModel.Status == "1" && priceModel.Message == "OK")
                    {
                        priceModel.Price.CoinBtc = Math.Round(priceModel.Price.CoinBtc ?? -1, 8);
                        priceModel.Price.CoinUsd = Math.Round(priceModel.Price.CoinUsd ?? -1, 2);
                        _logger.LogInformation($"ETC price updated. Result model: {JsonConvert.SerializeObject(priceModel)}");
                        return priceModel;
                    }
                    _logger.LogError($"ETC price update failed. Response model: {JsonConvert.SerializeObject(priceModel)}");
                    await Task.Delay(500 * (i + 1));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting ETC price.");
            }
            return null;
        }

        public async Task<double> GetEtcWalletBalanceAsync(string wallet)
        {
            try
            {
                for (int i = 0; i < maxTries; i++)
                {
                    _logger.LogInformation($"Getting ETC wallet balance for {wallet}, attempt {i + 1}.");
                    var walletBalanceModel = await _clientService.GetWalletBalanceAsync(_etcWalletAddress);
                    if (!walletBalanceModel.Result.Contains("Invalid address hash"))
                    {
                        var balance = Math.Round(float.Parse(walletBalanceModel.Result) / _divisor, 4);
                        _logger.LogInformation($"ETC wallet balance for {wallet} is {balance}. Result model: {JsonConvert.SerializeObject(walletBalanceModel)}");
                        return balance;
                    }
                    _logger.LogInformation($"ETC wallet balance for {wallet} is invalid. Response model: {JsonConvert.SerializeObject(walletBalanceModel)}");
                    await Task.Delay(500 * (i + 1));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting wallet balance.");
            }
            return -1;
        }
    }
}
