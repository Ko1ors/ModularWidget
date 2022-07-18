using Newtonsoft.Json;
using System;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class Status
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("elapsed")]
        public string Elapsed { get; set; }

        [JsonProperty("credit_count")]
        public int CreditCount { get; set; }
    }
}
