﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoinMarketCapPortfolioModule.Models.API
{
    public class PortfolioResponse<T> where T : class
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}
