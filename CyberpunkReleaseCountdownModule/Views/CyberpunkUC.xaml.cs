using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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

        private DateTime releaseDate = new DateTime(2020,12,10,0,0,0);

        public CyberpunkUC()
        {
            InitializeComponent();
            SetBaseValues();
            var timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimeEvent;
            timer.Start();
        }

        private void OnTimeEvent(object sender, EventArgs e)
        {
            var time = releaseDate.Subtract(DateTime.Now);
            UpdateTime(time);
        }

        private void UpdateTime(TimeSpan time)
        {
            Days = time.Days.ToString();
            Hours = time.Hours.ToString();
            Minutes = time.Minutes.ToString();
            Seconds = time.Seconds.ToString();
        }

        private void SetBaseValues()
        {
            Days = "31";
            Hours = "12";
            Minutes = "30";
            Seconds = "45";
        }
    }
}
