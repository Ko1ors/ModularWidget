﻿using ModularWidget.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCModule.Data
{
    public class EtcPrice : Root
    {
        [JsonProperty("result")]
        public EtcPriceResult Result;
    }

    public class EtcPriceResult
    {
        [JsonProperty("ethbtc")]
        public string Ethbtc;

        [JsonProperty("ethbtc_timestamp")]
        public string EthbtcTimestamp;

        [JsonProperty("ethusd")]
        public string Ethusd;

        [JsonProperty("ethusd_timestamp")]
        public string EthusdTimestamp;
    }
}
