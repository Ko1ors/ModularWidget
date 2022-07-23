using Newtonsoft.Json;

namespace ETHModule.Data
{
    public class EthGasPriceResult
    {
        [JsonProperty("LastBlock")]
        public string LastBlock;

        [JsonProperty("SafeGasPrice")]
        public string SafeGasPrice;

        [JsonProperty("ProposeGasPrice")]
        public string ProposeGasPrice;

        [JsonProperty("FastGasPrice")]
        public string FastGasPrice;
    }
}
