using Newtonsoft.Json;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class PieChart
    {
        [JsonProperty("holdings")]
        public double Holdings { get; set; }

        [JsonProperty("cryptoId")]
        public int CryptoId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("holdingsPercent")]
        public double HoldingsPercent { get; set; }
    }
}
