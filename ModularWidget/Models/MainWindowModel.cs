using ModularWidget.Common.Models;
using System.Windows;

namespace ModularWidget.Models
{
    public class MainWindowModel : ModelBase
    {
        private bool _showLogo;
        private bool _moduleFound;

        public bool ShowLogo
        {
            get { return _showLogo; }
            set 
            {
                _showLogo = value;
                OnPropertyChanged(nameof(ShowLogo));
                OnPropertyChanged(nameof(ShowLogoVisibility));
            }
        }

        public bool ModuleFound
        {
            get { return _moduleFound; }
            set
            {
                _moduleFound = value;
                OnPropertyChanged(nameof(ModuleFound));
                OnPropertyChanged(nameof(NoModuleFoundVisibility));
            }
        }

        public Visibility ShowLogoVisibility => ShowLogo ? Visibility.Visible : Visibility.Collapsed;

        public Visibility NoModuleFoundVisibility => ModuleFound ? Visibility.Collapsed : Visibility.Visible; 
    }
}
