using ETHModule.Data;
using ETHModule.Settings;
using ModularWidget;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public class EthService : IETHService
    {
        private const int maxTries = 5;

        private readonly AppSettings _appSettings;

        public EthService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<ETHCompositeModel> GetDataAsync()
        {
            var model = new ETHCompositeModel();
            model.EthPrice = await GetPriceAsync();
            model.EthGasPrice = await GetGasPriceAsync();
            model.AvgBlockReward = await GetAvgBlockRewardAsync(int.Parse(model.EthGasPrice.Result.LastBlock));
            if (!string.IsNullOrEmpty(_appSettings.Get<string>(Constants.Parameters.Wallet)))
                model.WalletBalance = await GetWalletBalanceAsync();
            return model;
        }

        public Task<EthPrice> GetPriceAsync()
        {
            var key = _appSettings.Get<string>(Constants.Parameters.ApiKey);
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetPrice(key);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    return Task.FromResult(result);
                }
                Thread.Sleep(500 * (i + 1));
            }
            return Task.FromResult(default(EthPrice));
        }

        public Task<EthGasPrice> GetGasPriceAsync()
        {
            var key = _appSettings.Get<string>(Constants.Parameters.ApiKey);
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetGasPrice(key);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    return Task.FromResult(result);
                }
                Thread.Sleep(500 * (i + 1));
            }
            return Task.FromResult(default(EthGasPrice));
        }

        public Task<double> GetAvgBlockRewardAsync(int lastBlock)
        {
            double blockreward = 0;
            bool success;
            int i;
            string key = _appSettings.Get<string>(Constants.Parameters.ApiKey);
            for (i = 0; i < 10; i++)
            {
                success = false;
                for (int j = 0; j < maxTries; j++)
                {
                    var result = EthRequest.GetBlockReward(key, lastBlock--.ToString());
                    if (result.Status != "0" && result.Message != "NOTOK")
                    {
                        success = true;
                        blockreward += double.Parse(result.Result.BlockReward) / 1000000000000000000;
                        break;
                    }
                    Thread.Sleep(500 * (j + 1));
                }
                if (!success)
                    return Task.FromResult(default(double));
            }
            return Task.FromResult(Math.Round(blockreward / i, 5));
        }

        public Task<double> GetWalletBalanceAsync()
        {
            var key = _appSettings.Get<string>(Constants.Parameters.ApiKey);
            var wallet = _appSettings.Get<string>(Constants.Parameters.Wallet);
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetWalletBalance(key, wallet);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    return Task.FromResult(Math.Round(double.Parse(result.Result) / 1000000000000000000, 5));
                }
                Thread.Sleep(500 * (i + 1));
            }
            return Task.FromResult(default(double));
        }
    }
}
