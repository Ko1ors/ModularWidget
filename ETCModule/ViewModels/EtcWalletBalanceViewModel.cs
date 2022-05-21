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

        public string WalletBalanceText => Result is null ?
            "ーー ETC ❙ $ーー" : $"{Result.WalletBalance.ToString().Replace(",", ".")} ETC ❙ ${Math.Round(Result.WalletBalance * Result.Price.CoinUsd, 2).ToString().Replace(",", ".")}";

        public EtcWalletBalanceViewModel(IEtcService etcService)
        {
            _etcService = etcService;
            _etcService.EtcUpdated += r => { Result = r; };
        }

    }
}
