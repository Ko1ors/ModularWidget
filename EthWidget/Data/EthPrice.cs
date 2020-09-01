using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthWidget.Data
{
    public class EthPrice : Root
    {
        [JsonProperty("result")]
        public EthPriceResult Result;
    }
}
