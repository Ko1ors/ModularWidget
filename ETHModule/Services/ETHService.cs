using ETHModule.Data;
using System;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public class EthService : IETHService
    {
        private const int maxTries = 5;

        public async Task<ETHCompositeModel> GetDataAsync(string apiKey, string wallet = "",
            bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false)
        {
            var model = new ETHCompositeModel();
            if (!ignorePrice)
                model.EthPrice = await GetPriceAsync(apiKey);
            if (!ignoreGas)
                model.EthGasPrice = await GetGasPriceAsync(apiKey);
            if (!ignoreBlockReward)
                model.AvgBlockReward = await GetAvgBlockRewardAsync(apiKey, int.Parse(model.EthGasPrice.Result.LastBlock));
            if (!string.IsNullOrEmpty(wallet))
                model.WalletBalance = await GetWalletBalanceAsync(apiKey, wallet);
            return model;
        }

        public async Task<EthPrice> GetPriceAsync(string apiKey)
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetPrice(apiKey);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    return result;
                }
                await Task.Delay(500 * (i + 1));
            }
            return default;
        }

        public async Task<EthGasPrice> GetGasPriceAsync(string apiKey)
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetGasPrice(apiKey);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    return result;
                }
                await Task.Delay(500 * (i + 1));
            }
            return default;
        }

        public async Task<double> GetAvgBlockRewardAsync(string apiKey, int lastBlock)
        {
            double blockreward = 0;
            bool success;
            int i;
            for (i = 0; i < 10; i++)
            {
                success = false;
                for (int j = 0; j < maxTries; j++)
                {
                    var result = EthRequest.GetBlockReward(apiKey, lastBlock--.ToString());
                    if (result.Status != "0" && result.Message != "NOTOK")
                    {
                        success = true;
                        blockreward += double.Parse(result.Result.BlockReward) / 1000000000000000000;
                        break;
                    }
                    await Task.Delay(500 * (j + 1));
                }
                if (!success)
                    return default;
            }
            return Math.Round(blockreward / i, 5);
        }

        public async Task<double> GetWalletBalanceAsync(string apiKey, string wallet)
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetWalletBalance(apiKey, wallet);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    return Math.Round(double.Parse(result.Result) / 1000000000000000000, 5);
                }
                await Task.Delay(500 * (i + 1));
            }
            return default;
        }
    }
}
