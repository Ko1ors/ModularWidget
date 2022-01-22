using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FearGreedIndexModule.Models
{
    public class FearGreedIndex : BaseModel
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
