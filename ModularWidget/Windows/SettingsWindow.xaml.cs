using System.Diagnostics;
using System.Windows;

namespace ModularWidget
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _appSettings;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _appSettings = settings;
            DataContext = _appSettings.Menus;

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
    }
}
