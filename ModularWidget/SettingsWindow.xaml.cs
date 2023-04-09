using ModularWidget.Models;
using ModularWidget.Services;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace ModularWidget
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private static readonly string ThemeFolderPath = "./Themes";

        private readonly AppSettings _appSettings;
        private readonly IThemeService _themeService;
        private readonly AppSettingsModel _context;
        private readonly SettingsParameter _themeParameter;
        private ThemeConfigModel _activeTheme;

        public SettingsWindow(AppSettings settings, IThemeService themeService)
        {
            InitializeComponent();
            _appSettings = settings;
            _themeService = themeService;

            var themes = _themeService.GetAllConfigurations(ThemeFolderPath).Select(t => new ThemeConfigModel(t)).ToList();
            _themeParameter = _appSettings.GetMenu(Constants.Menu.MenuKey).Get(Constants.Parameters.ThemeUri);
            _activeTheme = themes.First(t => t.Active);
            _context = new AppSettingsModel()
            {
                Menus = _appSettings.Menus,
                Themes = themes
            };
            DataContext = _context;

            Left = SystemParameters.PrimaryScreenWidth - this.Width * 1.15;
            Top = SystemParameters.PrimaryScreenHeight * 0.1;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset unsaved changes
            _appSettings.Reset();
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.Save();
            Close();
        }

        private void SaveRestart_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.Save();
            Process.Start(System.Environment.ProcessPath);
            Application.Current.Shutdown();
        }

        private void StackPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _activeTheme.Active = false;
            _activeTheme = ((StackPanel)sender).Tag as ThemeConfigModel;
            _activeTheme.Active = true;
            _themeParameter.Value = _activeTheme.RelativeUri;
            _themeService.LoadTheme(_activeTheme.RelativeUri);
        }
    }
}
