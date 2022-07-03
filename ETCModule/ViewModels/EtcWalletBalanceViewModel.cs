using ETCModule.Models;
using ETCModule.Services;
using System;

namespace ETCModule.ViewModels
{
    public class EtcWalletBalanceViewModel : ViewModelBase
    {
        private readonly IEtcService _etcService;
        private EtcCompositeResult _result;

        public EtcCompositeResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(WalletBalanceText));
            }
        }

        public string WalletBalanceText
        {
            get
            {
                if (Result is null || Result.WalletBalance < 0)
                    return "ーー ETC ❙ $ーー";

                string priceText = $"{Result.WalletBalance.ToString().Replace(",", ".")} ETC ❙";

                if (Result.Price.CoinUsd.HasValue && Result.Price.CoinUsd.Value >= 0)
                    priceText += $" ${Math.Round(Result.WalletBalance * Result.Price.CoinUsd.Value, 2).ToString().Replace(",", ".")}";
                else
                    priceText += " $ーー";
                return priceText;
            }
        }

        public EtcWalletBalanceViewModel(IEtcService etcService)
        {
            _etcService = etcService;
            _etcService.EtcUpdated += r => { Result = r; };
        }

    }
}
