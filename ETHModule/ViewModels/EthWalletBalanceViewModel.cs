using ETHModule.Data;
using ETHModule.Services;
using ModularWidget.Common.ViewModels;
using System;

namespace ETHModule.ViewModels
{
    public class EthWalletBalanceViewModel : ViewModelBase
    {
        private ETHCompositeModel _ethComposite;

        public ETHCompositeModel EthComposite
        {
            get { return _ethComposite; }
            set
            {
                _ethComposite = value;
                OnPropertyChanged(nameof(EthWalletBalanceText));
            }
        }

        public string EthWalletBalanceText
        {
            get
            {
                if (EthComposite is null)
                    return "$ーー ❙ ーー BTC";

                var priceString = EthComposite.EthPrice != null ? $"❙ ${Math.Round(double.Parse(EthComposite.EthPrice.Result.Ethusd.Replace('.', ',')) * EthComposite.WalletBalance, 2).ToString().Replace(',', '.')}" : "";
                return $"{EthComposite.WalletBalance.ToString().Replace(",", ".")} ETH " + priceString;
            }
        }

        public EthWalletBalanceViewModel(IETHService ethService)
        {
            ethService.EthUpdated += EthServiceEthUpdated;
        }

        private void EthServiceEthUpdated(ETHCompositeModel value)
        {
            EthComposite = value;
        }
    }
}
