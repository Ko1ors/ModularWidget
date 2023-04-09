using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ModularWidget.UserControls
{
    /// <summary>
    /// Interaction logic for ThemeColorsUC.xaml
    /// </summary>
    public partial class ThemeColorsUC : UserControl
    {
        public static readonly DependencyProperty ColorsProperty =
        DependencyProperty.Register(nameof(Colors), typeof(IEnumerable<string>), typeof(ThemeColorsUC),
        new PropertyMetadata(new List<string> { "Red", "Yellow", "Green", "Blue" }));

        public IEnumerable<string> Colors
        {
            get { return (IEnumerable<string>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        public ThemeColorsUC()
        {
            InitializeComponent();
        }
    }
}
