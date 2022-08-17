using Newtonsoft.Json;

namespace ETHModule.Data
{
    public abstract class Root
    {
        [JsonProperty("status")]
        public string Status;

        [JsonProperty("message")]
        public string Message;
    }
}
