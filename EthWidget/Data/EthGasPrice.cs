using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthWidget.Data
{
    class EthGasPrice : Root
    {
        [JsonProperty("result")]
        public EthGasPriceResult Result;
    }
}
