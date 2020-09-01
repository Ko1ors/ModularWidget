using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EthWidget
{
    public delegate void Notify();
    public static class Manager
    {
        private static Timer timer;
        public static event Notify Completed;
        public static void Start()
        {
            if(timer is null)
            {
                GetETHInformation();
                SetTimer();        
            }
        }

        private static void GetETHInformation()
        {
            OnComplete();
        }

        private static void SetTimer()
        {
            timer = new Timer(AppSettings.updateTime * 60 * 1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            GetETHInformation();
        }

        private static void OnComplete()
        {
            Completed?.Invoke();
        }
    }
}
