using Newtonsoft.Json;
using System.Collections.Generic;

namespace ETCModule.Models
{
    class EtcWalletsResult : ResultBase
    {
        [JsonProperty("result")]
        public List<EtcWalletResult> Result;
    }

    public class EtcWalletResult
    {
        [JsonProperty("address")]
        public string Address;

        [JsonProperty("balance")]
        public string Balance;

        [JsonProperty("stale")]
        public bool Stale;
    }
}
