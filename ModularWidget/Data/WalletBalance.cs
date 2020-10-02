using Newtonsoft.Json;

namespace ModularWidget.Data
{
    public class WalletBalance : Root
    {
        [JsonProperty("result")]
        public string Result;
    }
}
