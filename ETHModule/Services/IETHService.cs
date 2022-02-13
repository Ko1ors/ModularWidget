using ETHModule.Data;
using System.Threading.Tasks;

namespace ETHModule.Services
{
    public interface IETHService
    {
        Task<ETHCompositeModel> GetDataAsync();

        Task<EthPrice> GetPriceAsync();

        Task<EthGasPrice> GetGasPriceAsync();

        Task<double> GetAvgBlockRewardAsync(int lastBlock);

        Task<double> GetWalletBalanceAsync();
    }
}
