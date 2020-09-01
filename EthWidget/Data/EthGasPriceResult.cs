using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthWidget.Data
{
    public class EthGasPriceResult
    {
        [JsonProperty("LastBlock")]
        public string LastBlock;

        [JsonProperty("SafeGasPrice")]
        public string SafeGasPrice;

        [JsonProperty("ProposeGasPrice")]
        public string ProposeGasPrice;

        [JsonProperty("FastGasPrice")]
        public string FastGasPrice;
    }
}
