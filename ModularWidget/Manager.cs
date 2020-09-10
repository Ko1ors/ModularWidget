using ModularWidget.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Documents;

namespace ModularWidget
{
    public delegate void Notify();
    public static class Manager
    {
        private static System.Timers.Timer timer;
        private static int maxTries = 5;
        private static Object inProgress = new object();

        public static event Notify Completed;
        public delegate void RegionHandler(string regName);
        public static event RegionHandler RegionRequested;
        public static event RegionHandler RegionCreated;

        public static DateTime lastUpdate;
        public static EthPrice lastEthPrice;
        public static EthGasPrice lastGasPrice;
        public static double lastWalletBalance;
        public static double lastAvgBlockReward;
        public static DateTime nextUpdate;
        public static void Start()
        {
            if(timer is null)
            {
                nextUpdate = DateTime.Now.AddMinutes(AppSettings.updateTime);
                Task.Run(() => GetETHInformation());
                SetTimer();        
            }
        }

        private static void GetETHInformation()
        {
            lock (inProgress)
            {
                if (!TryGetEthPrice())
                    return;
                if (!TryGetEthGasPrice())
                    return;
                if (!TryGetAvgBlockReward(int.Parse(lastGasPrice.Result.LastBlock)))
                    return;
                if (!String.IsNullOrEmpty(AppSettings.ethWallet))
                    if (!TryGetWalletBalance())
                        return;
                lastUpdate = DateTime.Now;
                OnComplete();
            }
        }

        private static bool TryGetEthPrice()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetPrice(AppSettings.ethApiKey);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    lastEthPrice = result;
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private static bool TryGetEthGasPrice()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetGasPrice(AppSettings.ethApiKey);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    lastGasPrice = result;
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private static bool TryGetAvgBlockReward(int lastblock)
        {
            double blockreward = 0;
            bool success;
            int i;
            for (i = 0; i < 10; i++)
            {
                success = false;
                for (int j = 0; j < maxTries; j++)
                {
                    var result = EthRequest.GetBlockReward(AppSettings.ethApiKey,lastblock--.ToString());
                    if (result.Status != "0" && result.Message != "NOTOK")
                    {
                        success = true;
                        blockreward += Double.Parse(result.Result.BlockReward) / 1000000000000000000;
                        break;
                    }
                    Thread.Sleep(500 * (j +1));
                }
                if (!success)
                    return false;
            }
            lastAvgBlockReward = Math.Round(blockreward / i,5);
            return true;
        }

        private static bool TryGetWalletBalance()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EthRequest.GetWalletBalance(AppSettings.ethApiKey,AppSettings.ethWallet);
                if (result.Status != "0" && result.Message != "NOTOK")
                {
                    lastWalletBalance = Math.Round(double.Parse(result.Result) / 1000000000000000000,5);
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private static void SetTimer()
        {
            timer = new System.Timers.Timer(AppSettings.updateTime * 60 * 1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            nextUpdate = DateTime.Now.AddMinutes(AppSettings.updateTime);
            Task.Run(() => GetETHInformation());
        }

        private static void OnComplete()
        {
            Completed?.Invoke();
        }

        public static void RegionRequest(string regName)
        {
            RegionRequested?.Invoke(regName);
        }

        public static void RegionCreate(string regName)
        {
            RegionCreated.Invoke(regName);
        }

        public static void ThrowException()
        {
            throw new Exception("It is work");
        }
    }
}
