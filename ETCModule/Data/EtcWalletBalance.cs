using ModularWidget.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCModule.Data
{
    public class EtcWalletBalance : Root
    {
        [JsonProperty("result")]
        public string Result;
    }
}
