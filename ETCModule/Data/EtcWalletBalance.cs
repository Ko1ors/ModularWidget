using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCModule.Data
{
    public class EtcWalletBalance
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc;

        [JsonProperty("result")]
        public string Result;

        [JsonProperty("id")]
        public int Id;
    }
}
