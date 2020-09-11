using ETCModule.Data;
using ModularWidget;
using System;
using System.Threading;

namespace ETCModule.Models
{
    public class EtcInformation
    {
        private int maxTries = 5;
        private readonly Object inProgress = new object();

        public event Notify Completed;

        public string lastWalletBalance;
        public EtcPrice lastEtcPrice;

        public void Start()
        {
            Update();
            Manager.UpdateRequested += Update;
        }

        private void Update()
        {
            lock (inProgress)
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.etcWalletAddress))
                    if (!TryGetEtcWalletBalance())
                        return;
                if (!TryGetEtcPrice())
                    return;
                OnComplete();
            }
        }

        private bool TryGetEtcPrice()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EtcRequest.GetPrice();
                if (result.Status == "1" && result.Message == "OK")
                {
                    lastEtcPrice = result;
                    lastEtcPrice.Result.Ethbtc = lastEtcPrice.Result.Ethbtc.Replace(".", ",");
                    lastEtcPrice.Result.Ethusd = lastEtcPrice.Result.Ethusd.Replace(".", ",");
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private bool TryGetEtcWalletBalance()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = EtcRequest.GetWalletBalance(Properties.Settings.Default.etcWalletAddress);
                if (!result.Result.Contains("Invalid address hash"))
                {
                    lastWalletBalance = Convert.ToString(Int32.Parse(result.Result), 10);
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private void OnComplete()
        {
            Completed?.Invoke();
        }
    }
}
