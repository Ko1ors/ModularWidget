using ETHModule.Data;
using ETHModule.Services;
using ModularWidget.Common.ViewModels;

namespace ETHModule.ViewModels
{
    public class EthPriceViewModel : ViewModelBase
    {
        private EthPrice _ethPrice;

        public EthPrice EthPrice
        {
            get { return _ethPrice; }
            set
            {
                _ethPrice = value;
                OnPropertyChanged(nameof(EthPriceText));
            }
        }

        public string EthPriceText
        {
            get
            {
                if (EthPrice is null)
                    return "$ーー ❙ ーー BTC";

                return $"${EthPrice.Result.EthUsdRounded} ❙ {EthPrice.Result.EtcBtcRounded} BTC";
            }
        }

        public EthPriceViewModel(IETHService ethService)
        {
            ethService.EthPriceUpdated += EthServicePriceUpdated;
        }

        private void EthServicePriceUpdated(EthPrice value)
        {
            EthPrice = value;
        }
    }
}
