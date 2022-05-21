using Newtonsoft.Json;
using System.Collections.Generic;

namespace ETCModule.Models
{
    public class EtcWalletsResult : ResultBase
    {
        [JsonProperty("result")]
        public List<EtcWallet> EtcWallets;
    }

    public class EtcWallet
    {
        [JsonProperty("address")]
        public string Address;

        [JsonProperty("balance")]
        public string Balance;

        [JsonProperty("stale")]
        public bool Stale;
    }
}
