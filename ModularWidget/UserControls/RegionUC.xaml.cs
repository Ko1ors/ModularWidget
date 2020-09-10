using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ModularWidget.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RegionUC.xaml
    /// </summary>
    public partial class RegionUC : UserControl, INotifyPropertyChanged
    {
        private string regName;
        public string RegionName
        {
            get { return regName; }
            set
            {
                regName = value;
                OnPropertyChanged();
            }
        }
        public RegionUC()
        {
            DataContext = this;
            InitializeComponent();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
