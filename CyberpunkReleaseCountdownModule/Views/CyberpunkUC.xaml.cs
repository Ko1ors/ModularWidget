using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CyberpunkReleaseCountdownModule.Views
{
    /// <summary>
    /// Логика взаимодействия для CyberpunkUC.xaml
    /// </summary>
    public partial class CyberpunkUC : UserControl
    {
        public static readonly DependencyProperty DaysProperty = DependencyProperty.Register("DaysText", typeof(string), typeof(CyberpunkUC), new FrameworkPropertyMetadata("DaysText"));

        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register("HoursText", typeof(string), typeof(CyberpunkUC), new FrameworkPropertyMetadata("HoursText"));

        public static readonly DependencyProperty MinsProperty = DependencyProperty.Register("MinsText", typeof(string), typeof(CyberpunkUC), new FrameworkPropertyMetadata("MinsText"));

        public static readonly DependencyProperty SecsProperty = DependencyProperty.Register("SecsText", typeof(string), typeof(CyberpunkUC), new FrameworkPropertyMetadata("SecsText"));
        private string Days
        {
            get
            {
                return (string)this.GetValue(DaysProperty);
            }
            set
            {
                this.SetValue(DaysProperty, value);
            }
        }

        private string Hours
        {
            get
            {
                return (string)this.GetValue(HoursProperty);
            }
            set
            {
                this.SetValue(HoursProperty, value);
            }
        }

        private string Minutes
        {
            get
            {
                return (string)this.GetValue(MinsProperty);
            }
            set
            {
                this.SetValue(MinsProperty, value);
            }
        }

        private string Seconds
        {
            get
            {
                return (string)this.GetValue(SecsProperty);
            }
            set
            {
                this.SetValue(SecsProperty, value);
            }
        }


        public CyberpunkUC()
        {
            InitializeComponent();
            Days = "31";
            Hours = "12";
            Minutes = "30";
            Seconds = "45";
        }
    }
}
