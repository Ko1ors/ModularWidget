using Newtonsoft.Json;

namespace ETHModule.Data
{
    public class EthGasPrice : Root
    {
        [JsonProperty("result")]
        public EthGasPriceResult Result;
    }
}
