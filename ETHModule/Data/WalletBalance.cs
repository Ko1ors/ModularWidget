using Newtonsoft.Json;

namespace ETHModule.Data
{
    public class WalletBalance : Root
    {
        [JsonProperty("result")]
        public string Result;
    }
}
