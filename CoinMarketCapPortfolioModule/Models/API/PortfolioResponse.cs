using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class PortfolioResponse
    {
        [JsonProperty("data")]
        public List<Portfolio> Portfolios { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}
