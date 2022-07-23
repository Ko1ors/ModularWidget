using ETHModule.Data;
using System;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public delegate void Updated<T>(T value);

    public interface IETHService
    {
        event Updated<EthPrice> EthPriceUpdated;

        event Updated<EthGasPrice> GasPriceUpdated;

        event Updated<double> WalletBalanceUpdated;

        event Updated<double> AvgBlockRewardUpdated;

        event Updated<ETHCompositeModel> EthUpdated;

        Task<ETHCompositeModel> GetDataAsync(string apiKey, string wallet = "",
            bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false);

        Task<EthPrice> GetPriceAsync(string apiKey);

        Task<EthGasPrice> GetGasPriceAsync(string apiKey);

        Task<double> GetAvgBlockRewardAsync(string apiKey, int lastBlock);

        Task<double> GetWalletBalanceAsync(string apiKey, string wallet);

        Task StartAsync(string apiKey, TimeSpan? updateTime = null, string wallet = "",
            bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false);
    }
}
