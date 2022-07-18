using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class Portfolio
    {
        [JsonProperty("totalNum")]
        public int TotalNum { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("list")]
        public List<Cryptocurrency> Cryptocurrencies { get; set; }

        [JsonProperty("portfolioSourceId")]
        public string PortfolioSourceId { get; set; }

        [JsonProperty("portfolioName")]
        public string PortfolioName { get; set; }

        [JsonProperty("cryptoUnitPrice")]
        public double CryptoUnitPrice { get; set; }

        [JsonProperty("jobFlag")]
        public bool JobFlag { get; set; }

        [JsonProperty("totalAmount")]
        public double TotalAmount { get; set; }

        [JsonProperty("bgColor")]
        public string BgColor { get; set; }

        [JsonProperty("sortIndex")]
        public int SortIndex { get; set; }

        [JsonProperty("isMain")]
        public bool IsMain { get; set; }

        [JsonProperty("timeUpdated")]
        public DateTime TimeUpdated { get; set; }

        [JsonProperty("timeCreated")]
        public DateTime TimeCreated { get; set; }
    }
}
