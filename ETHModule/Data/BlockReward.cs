using Newtonsoft.Json;

namespace ETHModule.Data
{
    public class BlockReward : Root
    {
        [JsonProperty("result")]
        public BlockRewardResult Result;
    }
}
