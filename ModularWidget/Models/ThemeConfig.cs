using ModularWidget.Interfaces;
using System.Collections.Generic;

namespace ModularWidget.Models
{
    public class ThemeConfig : IThemeConfig
    {
        public string Name { get; set; }

        public string RelativeUri { get; set; }

        public bool Active { get; set; }

        public IEnumerable<string> Keys { get; set; }

        public IEnumerable<string> Colors { get; set; }
    }
}
