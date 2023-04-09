using ModularWidget.Models;
using System.Collections.Generic;

namespace ModularWidget.Services
{
    public interface IThemeService
    {
        void LoadDefaultThemeConfiguration();

        IEnumerable<ThemeConfig> GetAllConfigurations(string folderPath);

        void LoadTheme(string themeUri);

        string GetCurrentThemeName();
    }
}
