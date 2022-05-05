using ETCModule.Data;
using ETCModule.Settings;
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

        private readonly Double divisor = 1000000000000000000d;

        private System.Timers.Timer timer;

        private readonly AppSettings _appSettings;

        public EtcInformation(AppSettings settings)
        {
            _appSettings = settings;
        }

        public void Start()
        {
            LoadSettings();
            SetTimer();
            Update();
        }

        private void SetTimer()
        {
            var time = _appSettings.Get<int>(Constants.Parameters.UpdateTime);
            if (time <= 0)
                time = 5;
            timer = new System.Timers.Timer(time * 60 * 1000);
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
            etcWalletAddress = _appSettings.Get<string>(Constants.Parameters.Wallet);
            if (string.Equals(etcWalletAddress, "random", StringComparison.InvariantCultureIgnoreCase))
                etcWalletAddress = GetNonEmptyEtcWallet();
        }

        private void OnComplete()
        {
            Completed?.Invoke();
        }
    }
}
