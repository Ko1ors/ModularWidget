using Newtonsoft.Json;

namespace ModularWidget.Models
{
    public class SettingsParameter
    {
        [JsonConstructor]
        public SettingsParameter(string dataTypeName)
        {
            DataTypeName = dataTypeName;
        }

        public SettingsParameter()
        {
        }

        public string Key { get; set; }

        public string Name { get; set; }

        public bool Hidden { get; set; }

        public virtual string DataTypeName { get; }

        private object _value;

        public virtual object Value { get => _value; set { _value = value; ChangedValue = value; } }

        [JsonIgnore]
        public virtual object ChangedValue { get; set; }

        public void SaveChanges()
        {
            _value = ChangedValue;
        }
    }

    public class SettingsParameter<T> : SettingsParameter
    {
        private T value;

        public override object Value { get => value; set { this.value = (T)value; ChangedValue = value; } }

        public override string DataTypeName => typeof(T).FullName;

        public SettingsParameter(string key, T value)
        {
            Key = key;
            Name = key;
            Value = value;
        }

        public SettingsParameter(string key, string name, T value, bool hidden = false)
        {
            Key = key;
            Name = name;
            Value = value;
            Hidden = hidden;
        }
    }
}
