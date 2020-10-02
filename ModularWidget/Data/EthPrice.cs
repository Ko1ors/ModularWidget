using Newtonsoft.Json;

namespace ModularWidget.Data
{
    public class EthPrice : Root
    {
        [JsonProperty("result")]
        public EthPriceResult Result;
    }
}
