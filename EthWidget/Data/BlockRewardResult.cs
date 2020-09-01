using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthWidget.Data
{
    public class BlockRewardResult
    {
        [JsonProperty("blockNumber")]
        public string BlockNumber;

        [JsonProperty("timeStamp")]
        public string TimeStamp;

        [JsonProperty("blockMiner")]
        public string BlockMiner;

        [JsonProperty("blockReward")]
        public string BlockReward;

        [JsonProperty("uncles")]
        public List<object> Uncles;

        [JsonProperty("uncleInclusionReward")]
        public string UncleInclusionReward;
    }
}
