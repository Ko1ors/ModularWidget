using CryptoMarketCapModule.Models;
using Microsoft.Extensions.Logging;
using ModularWidget.Common.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMarketCapModule.Services
{
    public class CoinGeckoMarketCapService : IMarketCapService
    {
        private readonly string url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=100&page=1&sparkline=false";

        private readonly ModularHttpClient _httpClient;
        private readonly ILogger<CoinGeckoMarketCapService> _logger;

        public CoinGeckoMarketCapService(ModularHttpClient httpClient, ILogger<CoinGeckoMarketCapService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            SetupHttpClient();
        }

        private void SetupHttpClient()
        {
            _httpClient.HttpClient.DefaultRequestHeaders.Add("User-Agent", "Modular Widget");
        }

        public async Task<long> GetMarketCap()
        {
            try
            {
                _logger.LogInformation("CoinGeckoMarketCapService: Getting market cap");
                var result = await _httpClient.GetAsync(url);
                var coins = JsonConvert.DeserializeObject<List<Cryptocoin>>(result);
                long marketCap = coins.Sum(e => e.MarketCap);
                _logger.LogInformation($"CoinGeckoMarketCapService market cap received. Market cap is {marketCap}");
                return marketCap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting market cap");
                return 0;
            }
        }
    }
}
