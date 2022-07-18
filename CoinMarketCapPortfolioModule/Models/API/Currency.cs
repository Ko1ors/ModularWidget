using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class Cryptocurrency
    {
        [JsonProperty("cryptocurrencyId")]
        public int CryptocurrencyId { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("cryptoHoldings")]
        public double CryptoHoldings { get; set; }

        [JsonProperty("plPercentValue")]
        public double PlPercentValue { get; set; }

        [JsonProperty("plValue")]
        public double PlValue { get; set; }

        [JsonProperty("currentPrice")]
        public double CurrentPrice { get; set; }

        [JsonProperty("yesterdayChangePercent")]
        public double YesterdayChangePercent { get; set; }

        [JsonProperty("holdingsPercent")]
        public double HoldingsPercent { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("totalFee")]
        public int TotalFee { get; set; }

        [JsonProperty("buyAvgPrice")]
        public double BuyAvgPrice { get; set; }
    }
}
