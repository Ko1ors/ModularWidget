using Newtonsoft.Json;

namespace ETCModule.Models
{
    public class EtcWalletBalance : ResultBase
    {
        [JsonProperty("result")]
        public string Result;
    }
}
