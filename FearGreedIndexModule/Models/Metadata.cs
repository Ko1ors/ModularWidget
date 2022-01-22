using Newtonsoft.Json;

namespace FearGreedIndexModule.Models
{
    public class Metadata
    {
        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
