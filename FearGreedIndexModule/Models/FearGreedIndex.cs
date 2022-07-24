using ModularWidget.Common.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FearGreedIndexModule.Models
{
    public class FearGreedIndex : ModelBase
    {
        private string name;

        [JsonProperty("name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        [JsonProperty("data")]
        public ObservableCollection<IndexData> Data { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

    }
}
