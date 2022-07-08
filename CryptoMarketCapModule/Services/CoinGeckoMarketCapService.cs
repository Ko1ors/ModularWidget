﻿using CryptoMarketCapModule.Models;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<CoinGeckoMarketCapService> _logger;

        public CoinGeckoMarketCapService(ILogger<CoinGeckoMarketCapService> logger)
        {
            _logger = logger;
        }

        public async Task<long> GetMarketCap()
        {
            try
            {
                _logger.LogInformation("CoinGeckoMarketCapService: Getting market cap");
                var result = await Request.SendAsync(url);
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
