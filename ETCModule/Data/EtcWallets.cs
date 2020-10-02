using ModularWidget.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCModule.Data
{
    class EtcWallets : Root
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
