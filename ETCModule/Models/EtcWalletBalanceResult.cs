using Newtonsoft.Json;

namespace ETCModule.Models
{
    public class EtcWalletBalanceResult : ResultBase
    {
        [JsonProperty("result")]
        public string Result;
    }
}
