using ETHModule.Data;
using ETHModule.Services;
using ModularWidget.Common.ViewModels;

namespace ETHModule.ViewModels
{
    public class GasTrackerViewModel : ViewModelBase
    {
        private EthGasPrice _gasPrice;

        public EthGasPrice GasPrice
        {
            get { return _gasPrice; }
            set
            {
                _gasPrice = value;
                OnPropertyChanged(nameof(LowGasPrice));
                OnPropertyChanged(nameof(AverageGasPrice));
                OnPropertyChanged(nameof(HighGasPrice));
            }
        }

        public string LowGasPrice => $"{GasPrice?.Result?.SafeGasPrice ?? "--"} gwei";

        public string AverageGasPrice => $"{GasPrice?.Result?.ProposeGasPrice ?? "--"} gwei";

        public string HighGasPrice => $"{GasPrice?.Result?.FastGasPrice ?? "--"} gwei";


        public GasTrackerViewModel(IETHService ethService)
        {
            ethService.GasPriceUpdated += EthServiceGasTrackerUpdated;
        }

        private void EthServiceGasTrackerUpdated(EthGasPrice value)
        {
            GasPrice = value;
        }
    }
}
