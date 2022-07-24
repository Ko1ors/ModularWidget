using ETHModule.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public class EthService : IETHService
    {
        private const int maxTries = 5;

        private readonly TimeSpan _defaultUpdateTime = TimeSpan.FromMinutes(5);
        private readonly ILogger<EthService> _logger;
        private PeriodicTimer _timer;
        private Task _timerTask;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public event Updated<EthPrice> EthPriceUpdated;
        public event Updated<EthGasPrice> GasPriceUpdated;
        public event Updated<double> WalletBalanceUpdated;
        public event Updated<double> AvgBlockRewardUpdated;
        public event Updated<ETHCompositeModel> EthUpdated;

        public EthService(ILogger<EthService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(string apiKey, TimeSpan? updateTime = null, string wallet = "", bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false)
        {
            if (_timerTask is not null)
            {
                _cts.Cancel();
                await _timerTask;
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }

            _timer = new PeriodicTimer(updateTime ?? _defaultUpdateTime);
            await GetDataAsync(apiKey, wallet, ignorePrice, ignoreGas, ignoreBlockReward);
            _timerTask = RunPeriodicDataUpdatesAsync(apiKey, wallet, ignorePrice, ignoreGas, ignoreBlockReward);
        }

        private async Task RunPeriodicDataUpdatesAsync(string apiKey, string wallet = "", bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    await GetDataAsync(apiKey, wallet, ignorePrice, ignoreGas, ignoreBlockReward);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Periodic data updates canceled");
            }
        }

        public async Task<ETHCompositeModel> GetDataAsync(string apiKey, string wallet = "",
            bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false)
        {
            _logger.LogInformation($"GetDataAsync. Getting data from ETH API. API Key: {apiKey}. Wallet: {wallet}. Ignore price: {ignorePrice}. Ignore gas: {ignoreGas}. Ignore block reward: {ignoreBlockReward}");
            var model = new ETHCompositeModel();
            if (!ignorePrice)
                model.EthPrice = await GetPriceAsync(apiKey);
            if (!ignoreGas)
                model.EthGasPrice = await GetGasPriceAsync(apiKey);
            if (!ignoreBlockReward)
                model.AvgBlockReward = await GetAvgBlockRewardAsync(apiKey, int.Parse(model.EthGasPrice.Result.LastBlock));
            if (!string.IsNullOrEmpty(wallet))
                model.WalletBalance = await GetWalletBalanceAsync(apiKey, wallet);

            _logger.LogInformation($"GetDataAsync. Result model: {JsonConvert.SerializeObject(model)}");
            EthUpdated?.Invoke(model);
            return model;
        }

        public async Task<EthPrice> GetPriceAsync(string apiKey)
        {
            try
            {
                _logger.LogInformation($"Getting ETH price. Apikey: {apiKey}");
                for (int i = 0; i < maxTries; i++)
                {
                    _logger.LogInformation($"Getting ETH price. Try {i + 1} of {maxTries}");
                    var result = EthRequest.GetPrice(apiKey);
                    if (result.Status != "0" && result.Message != "NOTOK")
                    {
                        _logger.LogInformation($"ETH price received. Result model: {JsonConvert.SerializeObject(result)}");
                        EthPriceUpdated?.Invoke(result);
                        return result;
                    }
                    _logger.LogInformation($"ETH price not received. Result model: {JsonConvert.SerializeObject(result)}");
                    await Task.Delay(500 * (i + 1));
                }
                _logger.LogWarning($"ETH price not received. Max tries exceeded.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ETH price not received.");
            }
            return default;
        }

        public async Task<EthGasPrice> GetGasPriceAsync(string apiKey)
        {
            try
            {
                _logger.LogInformation($"Getting ETH gas price. Apikey: {apiKey}");
                for (int i = 0; i < maxTries; i++)
                {
                    _logger.LogInformation($"Getting ETH gas price. Try {i + 1} of {maxTries}");
                    var result = EthRequest.GetGasPrice(apiKey);
                    if (result.Status != "0" && result.Message != "NOTOK")
                    {
                        _logger.LogInformation($"ETH gas price received. Result model: {JsonConvert.SerializeObject(result)}");
                        GasPriceUpdated?.Invoke(result);
                        return result;
                    }
                    _logger.LogInformation($"ETH gas price not received. Result model: {JsonConvert.SerializeObject(result)}");
                    await Task.Delay(500 * (i + 1));
                }
                _logger.LogWarning($"ETH gas price not received. Max tries exceeded.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ETH gas price not received.");
            }
            return default;
        }

        public async Task<double> GetAvgBlockRewardAsync(string apiKey, int lastBlock)
        {
            try
            {
                double blockreward = 0;
                bool success;
                int i;
                _logger.LogInformation($"Getting ETH avg block reward. Apikey: {apiKey}. Last block: {lastBlock}");
                for (i = 0; i < 10; i++)
                {
                    success = false;
                    for (int j = 0; j < maxTries; j++)
                    {
                        _logger.LogInformation($"Getting ETH avg block reward. Block {i + 1} of 10; Try {j + 1} of {maxTries}");
                        var result = EthRequest.GetBlockReward(apiKey, lastBlock--.ToString());
                        if (result.Status != "0" && result.Message != "NOTOK")
                        {
                            success = true;
                            blockreward += double.Parse(result.Result.BlockReward) / 1000000000000000000;
                            _logger.LogInformation($"ETH avg block reward received. Block {i + 1} of 10;  Result model: {JsonConvert.SerializeObject(result)}");
                            break;
                        }
                        _logger.LogInformation($"ETH avg block reward not received. Block {i + 1} of 10; Result model: {JsonConvert.SerializeObject(result)}");
                        await Task.Delay(500 * (j + 1));
                    }
                    if (success)
                        continue;
                    _logger.LogWarning($"ETH avg block reward not received. Block {i + 1} of 10; Max tries exceeded.");
                    return default;
                }
                _logger.LogInformation($"ETH avg block reward received. Result: {blockreward}");
                double avgBlockReward = Math.Round(blockreward / i, 5);
                AvgBlockRewardUpdated?.Invoke(avgBlockReward);
                return avgBlockReward;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ETH avg block reward not received.");
                return default;
            }
        }

        public async Task<double> GetWalletBalanceAsync(string apiKey, string wallet)
        {
            try
            {
                _logger.LogInformation($"Getting ETH wallet balance. Apikey: {apiKey}. Wallet: {wallet}");
                for (int i = 0; i < maxTries; i++)
                {
                    var result = EthRequest.GetWalletBalance(apiKey, wallet);
                    if (result.Status != "0" && result.Message != "NOTOK")
                    {
                        double walletBalance = Math.Round(double.Parse(result.Result) / 1000000000000000000, 5);
                        _logger.LogInformation($"ETH wallet balance received. Result: {walletBalance}");
                        WalletBalanceUpdated?.Invoke(walletBalance);
                        return walletBalance;
                    }
                    await Task.Delay(500 * (i + 1));
                }
                return default;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ETH wallet balance not received.");
                return default;
            }
        }
    }
}
