using ETCModule.Models;
using System.Threading.Tasks;

namespace ETCModule.Services
{
    public interface IEtcClientService
    {
        Task<EtcPriceResult> GetPriceAsync();

        Task<EtcWalletBalanceResult> GetWalletBalanceAsync(string address);

        Task<EtcWalletsResult> GetRandomWalletsAsync(int count);
    }
}
