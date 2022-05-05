using ModularWidget.UserControls;
using System;

using System.Windows;

namespace ModularWidget
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppSettings _appSettings;
        
        public MainWindow(AppSettings settings)
        {
            InitializeComponent();
            _appSettings = settings;
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width * 1.1;
            this.Top = SystemParameters.PrimaryScreenHeight * 0.05;

            Manager.RegionRequested += CreateRegion;
            _appSettings.Load();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings(_appSettings);
            settingsWindow.ShowDialog();
        }

        private void CreateRegion(string regName)
        {
            RegionUC reg = new RegionUC();
            reg.RegionName = regName;
            AddRegion(reg);
            Manager.RegionCreate(reg.RegionName);
        }

        private void AddRegion(RegionUC reg)
        {
            stackPanelBlock.Children.Add(reg);
        }
    }
}
