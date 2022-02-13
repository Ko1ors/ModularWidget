using ETHModule.Data;
using ModularWidget;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public class ETHService : IETHService
    {
        private const int maxTries = 5;


        public async Task<ETHCompositeModel> GetDataAsync()
        {
            var model = new ETHCompositeModel();
            model.EthPrice = await GetPriceAsync();
            model.EthGasPrice = await GetGasPriceAsync();
            model.AvgBlockReward = await GetAvgBlockRewardAsync(int.Parse(model.EthGasPrice.Result.LastBlock));
            if (!string.IsNullOrEmpty(AppSettings.ethWallet))
                model.WalletBalance = await GetWalletBalanceAsync();
            return model;
        }

        public Task<EthPrice> GetPriceAsync()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetPrice(AppSettings.ethApiKey);
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
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetGasPrice(AppSettings.ethApiKey);
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
            for (i = 0; i < 10; i++)
            {
                success = false;
                for (int j = 0; j < maxTries; j++)
                {
                    var result = EthRequest.GetBlockReward(AppSettings.ethApiKey, lastBlock--.ToString());
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
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetWalletBalance(AppSettings.ethApiKey, AppSettings.ethWallet);
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
