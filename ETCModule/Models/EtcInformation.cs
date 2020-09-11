using ETCModule.Data;
using ModularWidget;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace ETCModule.Models
{
    public class EtcInformation
    {
        private int maxTries = 5;
        private readonly Object inProgress = new object();

        public event Notify Completed;

        [JsonProperty("etcWallet")]
        public string etcWalletAddress = "";

        public string lastWalletBalance;
        public EtcPrice lastEtcPrice;

        private readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "etc-settings.json";

        public void Start()
        {
            LoadSettings();
            Update();
            Manager.UpdateRequested += Update;
        }

        private void Update()
        {
            lock (inProgress)
            {
                if (!String.IsNullOrEmpty(etcWalletAddress))
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
                var result = EtcRequest.GetWalletBalance(etcWalletAddress);
                if (!result.Result.Contains("Invalid address hash"))
                {
                    lastWalletBalance = Convert.ToString(Math.Round(Convert.ToInt64(result.Result,16) / 1000000000000000000d,4));
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
                etcWalletAddress = File.ReadAllText(settingsPath);
            else
                File.WriteAllText(settingsPath, etcWalletAddress);
        }

        private void OnComplete()
        {
            Completed?.Invoke();
        }
    }
}
