using Microsoft.Extensions.Logging;
using ModularWidget.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace ModularWidget.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ILogger<ThemeService> _logger;
        private ThemeConfig _defaultConfig;
        private string _currentThemeName;

        public ThemeService(ILogger<ThemeService> logger)
        {
            _logger = logger;
        }

        private string FormatThemeName(string themeUri)
        {
            var name = Path.GetFileNameWithoutExtension(themeUri).Replace("Theme", string.Empty);
            // Add spaces between words
            return string.Join("", name.Select(ch => (char.IsUpper(ch) ? " " : "") + ch)).Trim();
        }

        public void LoadDefaultThemeConfiguration()
        {
            try
            {
                _defaultConfig = new ThemeConfig();
                var themeDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault();
                if (themeDictionary is null)
                {
                    _logger.LogError("Theme dictionary is not found");
                    return;
                }
                _currentThemeName = FormatThemeName(themeDictionary.Source.OriginalString);
                _defaultConfig.Keys = themeDictionary.Keys.Cast<string>().ToList();
                _logger.LogInformation("Default theme configuration loaded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading default theme configuration");
            }
        }

        public IEnumerable<ThemeConfig> GetAllConfigurations(string folderPath)
        {
            try
            {
                string[] resourceFiles = Directory.GetFiles(folderPath, "*.xaml");
                var themeConfigs = new List<ThemeConfig>();
                foreach (var resourceFile in resourceFiles)
                {
                    var themeConfig = new ThemeConfig();
                    var themeDictionary = new ResourceDictionary() { Source = new Uri(resourceFile, UriKind.Relative) };
                    // Compare keys with default configuration
                    themeConfig.Keys = themeDictionary.Keys.Cast<string>().ToList();
                    if (themeConfig.Keys.Except(_defaultConfig.Keys).Any())
                    {
                        _logger.LogWarning($"Theme configuration {resourceFile} has invalid keys");
                        continue;
                    }

                    themeConfig.Name = FormatThemeName(resourceFile);
                    themeConfig.Active = string.Equals(themeConfig.Name, _currentThemeName);
                    //themeConfig.Active = string.Equals(name, _currentThemeName);
                    themeConfig.RelativeUri = resourceFile;
                    themeConfig.Colors = themeDictionary.Values.OfType<Color>().Select(v => v.ToString())
                        .Where(v => v.StartsWith("#")).ToList();
                    themeConfigs.Add(themeConfig);
                }

                _logger.LogInformation("Loading theme configurations");
                return themeConfigs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading theme configurations");
                return Enumerable.Empty<ThemeConfig>();
            }
        }

        public void LoadTheme(string themeUri)
        {
            try
            {
                var resources = Application.Current.Resources;
                var themeDictionary = new ResourceDictionary() { Source = new Uri(themeUri, UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(themeDictionary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading theme {themeUri}");
            }
        }

        public string GetCurrentThemeName() => _currentThemeName;
    }
}
