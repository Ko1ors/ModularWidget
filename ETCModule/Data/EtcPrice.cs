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
        public double CoinBtc;

        [JsonProperty("coin_btc_timestamp")]
        public string CoinBtcTimestamp;

        [JsonProperty("coin_usd")]
        public double CoinUsd;

        [JsonProperty("coin_usd_timestamp")]
        public string CoinUsdTimestamp;
    }
}
