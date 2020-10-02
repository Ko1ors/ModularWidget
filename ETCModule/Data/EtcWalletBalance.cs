using ModularWidget.Data;
using Newtonsoft.Json;

namespace ETCModule.Data
{
    public class EtcWalletBalance : Root
    {
        [JsonProperty("result")]
        public string Result;
    }
}
