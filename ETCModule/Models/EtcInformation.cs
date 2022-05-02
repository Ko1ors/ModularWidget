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
        private readonly object inProgress = new object();

        public event Notify Completed;

        [JsonProperty("etcWallet")]
        public string etcWalletAddress = "";

        public string lastWalletBalance;
        public EtcPrice lastEtcPrice;

        private readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "etc-settings.json";
        private readonly Double divisor = 1000000000000000000d;

        private System.Timers.Timer timer;

        public void Start()
        {
            LoadSettings();
            SetTimer();
            Update();
        }

        private void SetTimer()
        {
            timer = new System.Timers.Timer(AppSettings.updateTime * 60 * 1000);
            timer.Elapsed += (s, e) => Update();
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void Update()
        {
            lock (inProgress)
            {
                if (!string.IsNullOrEmpty(etcWalletAddress))
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
                    lastEtcPrice.Result.CoinBtc = Math.Round(lastEtcPrice.Result.CoinBtc, 8);
                    lastEtcPrice.Result.CoinUsd = Math.Round(lastEtcPrice.Result.CoinUsd, 2);
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
                    lastWalletBalance = Convert.ToString(Math.Round(float.Parse(result.Result) / divisor, 4));
                    return true;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return false;
        }

        private string GetNonEmptyEtcWallet()
        {
            string wallet = null;
            while (string.IsNullOrEmpty(wallet))
            {
                wallet = EtcRequest.GetRandomWallets(10).Result.Find(e => !e.Balance.Equals(0) && Convert.ToDouble(e.Balance) / divisor > 1)?.Address;
            }
            return wallet;
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                etcWalletAddress = File.ReadAllText(settingsPath);
                switch (etcWalletAddress)
                {
                    case "default":
                        if (AppSettings.isLoaded)
                            etcWalletAddress = AppSettings.ethWallet;
                        else
                            etcWalletAddress = null;
                        break;
                    case "random":
                        etcWalletAddress = GetNonEmptyEtcWallet();
                        break;
                }
            }
            else
                File.WriteAllText(settingsPath, etcWalletAddress);
        }

        private void OnComplete()
        {
            Completed?.Invoke();
        }
    }
}
