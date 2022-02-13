using Newtonsoft.Json;

namespace ETHModule.Data
{
    public class EthPrice : Root
    {
        [JsonProperty("result")]
        public EthPriceResult Result;
    }
}
