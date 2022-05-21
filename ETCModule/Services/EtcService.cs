using ETCModule.Models;
using ETCModule.Settings;
using ModularWidget;
using System;
using System.Threading;

namespace ETCModule.Services
{
    public class EtcService : IEtcService
    {
        private readonly AppSettings _appSettings;
        private System.Timers.Timer _timer;
        private string _etcWalletAddress;
        private int maxTries = 5;
        private const double _divisor = 1000000000000000000d;

        public event Updated<EtcPrice> EtcPriceUpdated;
        public event Updated<double> WalletBalanceUpdated;
        public event Updated<EtcCompositeResult> EtcUpdated;

        public EtcService(AppSettings settings)
        {
            _appSettings = settings;
        }

        public void Start()
        {
            LoadWalletSettings();
            SetTimer();
            Update();
        }

        private void SetTimer()
        {
            var time = _appSettings.Get<int>(Constants.Parameters.UpdateTime);
            if (time <= 0)
                time = 5;
            _timer = new System.Timers.Timer(time * 60 * 1000);
            _timer.Elapsed += (s, e) => Update();
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void LoadWalletSettings()
        {
            _etcWalletAddress = _appSettings.Get<string>(Constants.Parameters.Wallet);
            if (string.Equals(_etcWalletAddress, "random", StringComparison.InvariantCultureIgnoreCase))
                _etcWalletAddress = GetNonEmptyEtcWallet();
        }

        private string GetNonEmptyEtcWallet()
        {
            string wallet = null;
            for (int i = 0; i < maxTries && string.IsNullOrEmpty(wallet); i++)
            {
                wallet = EtcRequest.GetRandomWallets(10).Result.Find(e => !e.Balance.Equals(0) && Convert.ToDouble(e.Balance) / _divisor > 1)?.Address;
            }
            return wallet;
        }

        private void Update()
        {
            var result = new EtcCompositeResult();
            if (!string.IsNullOrEmpty(_etcWalletAddress))
            {
                result.WalletBalance = GetEtcWalletBalance(_etcWalletAddress);
                WalletBalanceUpdated?.Invoke(result.WalletBalance);
            }
            result.EtcPrice = GetEtcPrice();
            EtcPriceUpdated?.Invoke(result.EtcPrice);
            EtcUpdated?.Invoke(result);
        }

        public EtcPrice GetEtcPrice()
        {
            for (int i = 0; i < maxTries; i++)
            {
                var priceModel = EtcRequest.GetPrice();
                if (priceModel.Status == "1" && priceModel.Message == "OK")
                {
                    priceModel.Result.CoinBtc = Math.Round(priceModel.Result.CoinBtc, 8);
                    priceModel.Result.CoinUsd = Math.Round(priceModel.Result.CoinUsd, 2);
                    return priceModel;
                }
                Thread.Sleep(500 * (i + 1));
            }
            return null;
        }

        public double GetEtcWalletBalance(string wallet)
        {
            for (int i = 0; i < maxTries; i++)
            {
                var walletBalanceModel = EtcRequest.GetWalletBalance(_etcWalletAddress);
                if (!walletBalanceModel.Result.Contains("Invalid address hash"))
                {
                    return Math.Round(float.Parse(walletBalanceModel.Result) / _divisor, 4);
                }
                Thread.Sleep(500 * (i + 1));
            }
            return -1;
        }
    }
}
