using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthWidget.Data
{
    class WalletBalance : Root
    {
        [JsonProperty("result")]
        public string Result;
    }
}
