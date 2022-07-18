using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class PortfolioStatistics
    {
        [JsonProperty("totalPlValue")]
        public double TotalPlValue { get; set; }

        [JsonProperty("totalPlpercentValue")]
        public double TotalPlpercentValue { get; set; }

        [JsonProperty("bestPlValue")]
        public double BestPlValue { get; set; }

        [JsonProperty("bestPlpercentValue")]
        public double BestPlpercentValue { get; set; }

        [JsonProperty("bestCryptoId")]
        public int BestCryptoId { get; set; }

        [JsonProperty("bestName")]
        public string BestName { get; set; }

        [JsonProperty("bestSymbol")]
        public string BestSymbol { get; set; }

        [JsonProperty("worstPlValue")]
        public double WorstPlValue { get; set; }

        [JsonProperty("worstPlpercentValue")]
        public double WorstPlpercentValue { get; set; }

        [JsonProperty("worstCryptoId")]
        public int WorstCryptoId { get; set; }

        [JsonProperty("worstName")]
        public string WorstName { get; set; }

        [JsonProperty("worstSymbol")]
        public string WorstSymbol { get; set; }

        [JsonProperty("pieCharts")]
        public List<PieChart> PieCharts { get; set; }

        [JsonProperty("portfolioSourceId")]
        public string PortfolioSourceId { get; set; }

        [JsonProperty("cryptoUnitPrice")]
        public double CryptoUnitPrice { get; set; }

        [JsonProperty("fiatUnitPrice")]
        public double FiatUnitPrice { get; set; }

        [JsonProperty("yesterdayBalancePercent")]
        public double? YesterdayBalancePercent { get; set; }

        [JsonProperty("yesterdayChangeBalance")]
        public double YesterdayChangeBalance { get; set; }

        [JsonProperty("currentTotalHoldings")]
        public double CurrentTotalHoldings { get; set; }
    }
}
