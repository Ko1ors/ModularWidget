using Newtonsoft.Json;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class PortfolioRequest
    {
        [JsonProperty("portfolioSourceId")]
        public string PortfolioSourceId { get; set; }

        [JsonProperty("cryptoUnit")]
        public int CryptoUnit { get; set; }

        [JsonProperty("fiatUnit")]
        public int FiatUnit { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }
}
