using ModularWidget.Common.Models;
using System.Collections.Generic;

namespace ModularWidget.Models
{
    public class AppSettingsModel : ModelBase
    {
        public IEnumerable<SettingsMenu> Menus { get; set; }

        public IEnumerable<ThemeConfigModel> Themes { get; set; }
    }
}
