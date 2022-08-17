using Microsoft.Extensions.Logging;
using ModularWidget.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ModularWidget
{
    public partial class AppSettings
    {
        private readonly ILogger<AppSettings> _logger;

        public bool IsLoaded { get; private set; }

        internal List<SettingsMenu> Menus { get; set; }

        public AppSettings(ILogger<AppSettings> logger)
        {
            _logger = logger;
        }

        public void Save()
        {
            try
            {
                // log
                _logger.LogInformation("Saving settings");
                // measure elapsed time
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // save changes in parameters 
                Menus.SelectMany(p => p.Parameters).ToList().ForEach(p => p.SaveChanges());

                string json = JsonConvert.SerializeObject(Menus, Formatting.Indented);
                File.WriteAllText(@"settings.json", json);
                stopwatch.Stop();
                _logger.LogInformation($"Settings saved. Elapsed time: {stopwatch.ElapsedMilliseconds} ms. Model: {JsonConvert.SerializeObject(Menus)}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving settings");
            }
        }

        public void Load()
        {
            try
            {
                // log
                _logger.LogInformation("Loading settings");
                // measure elapsed time
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                if (File.Exists(@"settings.json"))
                {
                    string json = File.ReadAllText(@"settings.json");
                    Menus = JsonConvert.DeserializeObject<List<SettingsMenu>>(json);
                }
                if (Menus is null)
                    Menus = new List<SettingsMenu>();

                IsLoaded = true;

                stopwatch.Stop();
                _logger.LogInformation($"Settings loaded. Elapsed time: {stopwatch.ElapsedMilliseconds} ms. Model: {JsonConvert.SerializeObject(Menus)}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error loading settings");
            }
        }

        public void Reset()
        {
            _logger.LogInformation("Resetting settings");
            Menus.SelectMany(p => p.Parameters).ToList().ForEach(p => p.ChangedValue = p.Value);
        }

        public bool AddOrUpdateMenu(SettingsMenu menu)
        {
            try
            {
                // log
                _logger.LogInformation($"Adding or updating {menu?.Name} menu. Menu model: {JsonConvert.SerializeObject(menu)}");
                var stopwatch = new Stopwatch();
                stopwatch.Start();

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
                stopwatch.Stop();
                _logger.LogInformation($"{menu?.Name} menu added or updated. Elapsed time: {stopwatch.ElapsedMilliseconds} ms.");

                Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error adding or updating {menu?.Name} menu.");
                return false;
            }
        }

        public bool AddOrUpdateParameter(string menuKey, SettingsParameter parameter)
        {
            try
            {
                // log
                _logger.LogInformation($"Adding or updating {parameter?.Name} parameter. Parameter model: {JsonConvert.SerializeObject(parameter)}");
                var stopwatch = new Stopwatch();
                stopwatch.Start();

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
                stopwatch.Stop();
                _logger.LogInformation($"{parameter?.Name} parameter added or updated. Elapsed time: {stopwatch.ElapsedMilliseconds} ms.");

                Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error adding or updating {parameter?.Name} parameter.");
                return false;
            }
        }

        public bool MenuExists(string key)
        {
            return GetMenu(key) != null;
        }

        public bool ParameterExists(string menuKey, string parameterKey)
        {
            return GetMenu(menuKey)?.Get(parameterKey) != null;
        }

        public SettingsMenu GetMenu(string key)
        {
            return Menus.FirstOrDefault(x => x.Key == key);
        }

        public SettingsParameter GetParameter(string menuKey, string parameterKey)
        {
            return GetMenu(menuKey)?.Get(parameterKey);
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
