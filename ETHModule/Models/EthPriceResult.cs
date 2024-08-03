using Newtonsoft.Json;
using System;
using System.Globalization;

namespace ETHModule.Data
{
    public class EthPriceResult
    {
        [JsonProperty("ethbtc")]
        public string Ethbtc;

        [JsonProperty("ethbtc_timestamp")]
        public string EthbtcTimestamp;

        [JsonProperty("ethusd")]
        public string Ethusd;

        public double EthBtcDouble => double.TryParse(Ethbtc, NumberStyles.Currency, CultureInfo.InvariantCulture, out double result) ? result : 0;

        public double EtcBtcRounded => Math.Round(EthBtcDouble, 8);

        public double EthUsdDouble => double.TryParse(Ethusd, NumberStyles.Currency, CultureInfo.InvariantCulture, out double result) ? result : 0;

        public double EthUsdRounded => Math.Round(EthUsdDouble, 2);

        [JsonProperty("ethusd_timestamp")]
        public string EthusdTimestamp;
    }
}
