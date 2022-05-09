using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModularWidget.Models
{
    public class SettingsMenu
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public List<SettingsParameter> Parameters { get; set; }

        public SettingsMenu(string key)
        {
            Key = key;
            Name = key;
            Parameters = new List<SettingsParameter>();
        }

        [JsonConstructor]
        public SettingsMenu(string key, string name)
        {
            Key = key;
            Name = name;
            Parameters = new List<SettingsParameter>();
        }

    }
}
