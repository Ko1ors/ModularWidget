using Newtonsoft.Json;
using System.Collections.Generic;

namespace ETCModule.Models
{
    class EtcWallets : ResultBase
    {
        [JsonProperty("result")]
        public List<EtcWalletsResult> Result;
    }

    public class EtcWalletsResult
    {
        [JsonProperty("address")]
        public string Address;

        [JsonProperty("balance")]
        public string Balance;

        [JsonProperty("stale")]
        public bool Stale;
    }
}
