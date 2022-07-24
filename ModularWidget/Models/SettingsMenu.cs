using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

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

        public T Get<T>(string parameterKey)
        {
            var parameter = Get(parameterKey);
            if (parameter is null)
                return default;
            return (T)Convert.ChangeType(parameter.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        public SettingsParameter Get(string parameterKey)
        {
            return Parameters?.Find(x => x.Key == parameterKey);
        }
    }
}
