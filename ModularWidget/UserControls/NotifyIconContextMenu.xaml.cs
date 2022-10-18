using ModularWidget.ViewModels;
using System.Windows.Controls;

namespace ModularWidget.UserControls
{
    /// <summary>
    /// Interaction logic for NotifyIconUC.xaml
    /// </summary>
    public partial class NotifyIconUC : ContextMenu
    {
        public NotifyIconUC()
        {
            InitializeComponent();
            DataContext = App.Resolve<NotifyIconViewModel>();
        }
    }
}
