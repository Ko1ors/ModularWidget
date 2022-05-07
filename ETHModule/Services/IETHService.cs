using ETHModule.Data;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public interface IETHService
    {
        Task<ETHCompositeModel> GetDataAsync(string apiKey, string wallet = "",
            bool ignorePrice = false, bool ignoreGas = false, bool ignoreBlockReward = false);

        Task<EthPrice> GetPriceAsync(string apiKey);

        Task<EthGasPrice> GetGasPriceAsync(string apiKey);

        Task<double> GetAvgBlockRewardAsync(string apiKey, int lastBlock);

        Task<double> GetWalletBalanceAsync(string apiKey, string wallet);
    }
}
