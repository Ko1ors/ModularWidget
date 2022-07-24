using ModularWidget.Common.Models;
using Newtonsoft.Json;
using System;

namespace FearGreedIndexModule.Models
{
    public class IndexData : ModelBase
    {
        private int value;
        private string valueClassification;
        private double timestamp;
        private int? timeUntilUpdate;

        [JsonProperty("value")]
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        [JsonProperty("value_classification")]
        public string ValueClassification
        {
            get
            {
                return valueClassification;
            }
            set
            {
                valueClassification = value;
                OnPropertyChanged("ValueClassification");
            }
        }

        [JsonProperty("timestamp")]
        public double Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
                OnPropertyChanged("TimeStamp");
                OnPropertyChanged("LastUpdateTime");
            }
        }

        [JsonProperty("time_until_update")]
        public int? TimeUntilUpdate
        {
            get
            {
                return timeUntilUpdate;
            }
            set
            {
                timeUntilUpdate = value;
                OnPropertyChanged("TimeUntilUpdate");
            }
        }

        public string LastUpdateTimeString => DateTime.UnixEpoch.AddSeconds(timestamp).ToString("dd/MM/yyyy");
    }
}
