using ETCModule.Models;
using System.Threading.Tasks;

namespace ETCModule.Services
{
    public delegate void Updated<T>(T value);

    public interface IEtcService
    {
        event Updated<EtcPriceResult> EtcPriceUpdated;

        event Updated<double> WalletBalanceUpdated;

        event Updated<EtcCompositeResult> EtcUpdated;

        Task StartAsync();

        Task<EtcPriceResult> GetEtcPriceAsync();

        Task<double> GetEtcWalletBalanceAsync(string wallet);
    }
}
