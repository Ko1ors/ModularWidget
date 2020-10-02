using Newtonsoft.Json;

namespace ModularWidget.Data
{
    public class EthGasPrice : Root
    {
        [JsonProperty("result")]
        public EthGasPriceResult Result;
    }
}
