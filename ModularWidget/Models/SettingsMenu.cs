using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
