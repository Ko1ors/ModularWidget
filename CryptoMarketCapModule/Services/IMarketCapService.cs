using System.Threading.Tasks;

namespace CryptoMarketCapModule.Services
{
    public interface IMarketCapService
    {
        Task<long> GetMarketCap();
    }
}
