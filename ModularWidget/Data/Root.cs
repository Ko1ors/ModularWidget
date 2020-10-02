using Newtonsoft.Json;

namespace ModularWidget.Data
{
    public abstract class Root
    {
        [JsonProperty("status")]
        public string Status;

        [JsonProperty("message")]
        public string Message;
    }
}
