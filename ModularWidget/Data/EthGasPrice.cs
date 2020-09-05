using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularWidget.Data
{
    public class EthGasPrice : Root
    {
        [JsonProperty("result")]
        public EthGasPriceResult Result;
    }
}
