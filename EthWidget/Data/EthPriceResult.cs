using Newtonsoft.Json;

namespace EthWidget.Data
{
    public class EthPriceResult
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
