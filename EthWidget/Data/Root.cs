using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthWidget.Data
{
    public abstract class Root
    {
        [JsonProperty("status")]
        public string Status;

        [JsonProperty("message")]
        public string Message;
    }
}
