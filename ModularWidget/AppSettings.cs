using ModularWidget.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ModularWidget
{
    public partial class AppSettings
    {
        public bool IsLoaded { get; private set; }

        internal List<SettingsMenu> Menus { get; set; }

        public void Save()
        {
            // save changes in parameters 
            Menus.SelectMany(p => p.Parameters).ToList().ForEach(p => p.SaveChanges());
            
            string json = JsonConvert.SerializeObject(Menus, Formatting.Indented);
            File.WriteAllText(@"settings.json", json);
        }

        public void Load()
        {
            if (File.Exists(@"settings.json"))
            {
                string json = System.IO.File.ReadAllText(@"settings.json");
                Menus = JsonConvert.DeserializeObject<List<SettingsMenu>>(json);
            }                
            if (Menus is null)
                Menus = new List<SettingsMenu>();

            IsLoaded = true;

        }

        public void Reset()
        {
            Menus.SelectMany(p => p.Parameters).ToList().ForEach(p => p.ChangedValue = p.Value);
        }
            
        public bool AddOrUpdateMenu(SettingsMenu menu)
        {
            var menuToUpdate = Menus.Find(x => x.Key == menu.Key);
            if (menuToUpdate is null)
            {
                Menus.Add(menu);
            }
            else
            {
                menuToUpdate.Name = menu.Name;
                menuToUpdate.Parameters = menu.Parameters;
            }
            Save();
            return true;
        }

        public bool AddOrUpdateParameter(string menuKey, SettingsParameter parameter)
        {
            var menu = Menus.Find(x => x.Key == menuKey);
            if (menu is null)
                return false;
            var parameterToUpdate = menu.Parameters.Find(x => x.Key == parameter.Key);
            if (parameterToUpdate is null)
            {
                menu.Parameters.Add(parameter);
            }
            else
            {
                if (parameter.DataTypeName == parameterToUpdate.DataTypeName)
                {
                    menu.Parameters.Remove(parameterToUpdate);
                    parameterToUpdate = new SettingsParameter(parameter.DataTypeName);
                    menu.Parameters.Add(parameterToUpdate);
                }
                parameterToUpdate.Name = parameter.Name;
                parameterToUpdate.Value = parameter.Value;
            }
            Save();
            return true;
        }

        public bool MenuExists(string key)
        {
            return GetMenu(key) != null;
        }

        public bool ParameterExists(string menuKey, string parameterKey)
        {
            return GetParameter(menuKey, parameterKey) != null;
        }

        public SettingsMenu GetMenu(string key)
        {
            return Menus.FirstOrDefault(x => x.Key == key);
        }

        public SettingsParameter GetParameter(string menuKey, string parameterKey)
        {
            return GetMenu(menuKey)?.Parameters?.Find(x => x.Key == parameterKey);
        }

        public T Get<T>(string menuKey, string parameterKey)
        {
            var parameter = GetParameter(menuKey, parameterKey);
            if (parameter is null)
                return default;
            return (T)Convert.ChangeType(parameter.Value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
