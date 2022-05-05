using ModularWidget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHModule.Settings
{
    internal static class SettingsExtension
    {
        public static T Get<T>(this AppSettings settings, string parameterKey)
        {
            return settings.Get<T>(Constants.Menu.MenuKey, parameterKey);
        }

        public static object Get(this AppSettings settings, string parameterKey)
        {
            return settings.GetParameter(Constants.Menu.MenuKey, parameterKey)?.Value;
        }
    }
}
