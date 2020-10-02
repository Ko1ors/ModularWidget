using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModularWidget.Data
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
