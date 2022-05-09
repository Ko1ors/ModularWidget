using CryptoMarketCapModule.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMarketCapModule.Services
{
    public class CoinGeckoMarketCapService : IMarketCapService
    {
        private readonly string url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=100&page=1&sparkline=false";

        public async Task<long> GetMarketCap()
        {
            try
            {
                var result = await Request.SendAsync(url);
                var coins = JsonConvert.DeserializeObject<List<Cryptocoin>>(result);
                return coins.Sum(e => e.MarketCap);
            }
            catch
            {
                return 0;
            }
        }
    }
}
