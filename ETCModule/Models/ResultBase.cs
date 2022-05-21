using Newtonsoft.Json;

namespace ETCModule.Models
{
    public abstract class ResultBase
    {
        [JsonProperty("status")]
        public string Status;

        [JsonProperty("message")]
        public string Message;
    }
}
