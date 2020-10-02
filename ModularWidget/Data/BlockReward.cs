using Newtonsoft.Json;

namespace ModularWidget.Data
{
    public class BlockReward : Root
    {
        [JsonProperty("result")]
        public BlockRewardResult Result;
    }
}
