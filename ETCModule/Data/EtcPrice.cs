using ModularWidget.Data;
using Newtonsoft.Json;

namespace ETCModule.Data
{
    public class EtcPrice : Root
    {
        [JsonProperty("result")]
        public EtcPriceResult Result;
    }

    public class EtcPriceResult
    {
        [JsonProperty("coin_btc")]
        public string Ethbtc { get; set; }

        [JsonProperty("coin_btc_timestamp")]
        public string EthbtcTimestamp { get; set; }

        [JsonProperty("coin_usd")]
        public string Ethusd { get; set; }

        [JsonProperty("coin_usd_timestamp")]
        public string EthusdTimestamp { get; set; }
    }
}
