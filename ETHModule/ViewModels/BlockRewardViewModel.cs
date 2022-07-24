using ETHModule.Services;
using ModularWidget.Common.ViewModels;

namespace ETHModule.ViewModels
{
    public class BlockRewardViewModel : ViewModelBase
    {
        private double _avgBlockReward;

        public double AverageBlockReward
        {
            get { return _avgBlockReward; }
            set
            {
                _avgBlockReward = value;
                OnPropertyChanged(nameof(AverageBlockRewardText));
            }
        }

        public string AverageBlockRewardText
        {
            get
            {
                if (AverageBlockReward is default(double))
                    return "$ーー ❙ ーー BTC";

                return $"{AverageBlockReward.ToString().Replace(",", ".")} ETH";
            }
        }

        public BlockRewardViewModel(IETHService ethService)
        {
            ethService.AvgBlockRewardUpdated += EthServiceBlockRewardUpdated;
        }

        private void EthServiceBlockRewardUpdated(double value)
        {
            AverageBlockReward = value;
        }
    }
}
