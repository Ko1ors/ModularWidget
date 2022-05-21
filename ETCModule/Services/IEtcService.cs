using ETCModule.Models;

namespace ETCModule.Services
{
    public delegate void Updated<T>(T value);

    public interface IEtcService
    {
        event Updated<EtcPrice> EtcPriceUpdated;

        event Updated<double> WalletBalanceUpdated;

        event Updated<EtcCompositeResult> EtcUpdated;

        void Start();

        EtcPrice GetEtcPrice();

        double GetEtcWalletBalance(string wallet);
    }
}
