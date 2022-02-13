using ETHModule.Services;
using ETHModule.UserControls;
using ModularWidget;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ETHModule
{
    public class LoadModule : IModule
    {
        IRegionManager regionManager;
        private IETHService _ethService;
        private Timer timer;
        private int regionsRequestedCount;
        private Dictionary<string, UserControl> userControls;
        private Dispatcher dispatcher;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager = containerProvider.Resolve<IRegionManager>();
            _ethService = containerProvider.Resolve<IETHService>();
            userControls = new Dictionary<string, UserControl>();
            dispatcher = Dispatcher.CurrentDispatcher;

            Manager.RegionCreated += Manager_RegionCreated;

            regionsRequestedCount = 3;
            Manager.RegionRequest(RegionsName.BlockRewards);
            Manager.RegionRequest(RegionsName.EthPrice);
            Manager.RegionRequest(RegionsName.GasTracker);
            if (!string.IsNullOrEmpty(AppSettings.ethWallet))
            {
                regionsRequestedCount++;
                Manager.RegionRequest(RegionsName.EthWallet);
            }
        }

        private void Manager_RegionCreated(string regName)
        {
            switch (regName)
            {
                case RegionsName.BlockRewards:
                    AddRegion(regName, new BlockRewardUC());
                    break;
                case RegionsName.EthPrice:
                    AddRegion(regName, new EthPriceUC());
                    break;
                case RegionsName.EthWallet:
                    AddRegion(regName, new EthWalletBalanceUC());
                    break;
                case RegionsName.GasTracker:
                    AddRegion(regName, new GasTrackerUC());
                    break;
            }
        }

        private void AddRegion(string name, UserControl userControl)
        {
            userControls[name] = userControl;
            regionManager.Regions[name].Add(userControl);
            if (--regionsRequestedCount <= 0)
            {
                Manager.RegionCreated -= Manager_RegionCreated;
                Task.Run(async () => await UpdateETHData());
                SetTimer();
            }
        }

        private void SetTimer()
        {
            timer = new Timer(AppSettings.updateTime * 60 * 1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
           _ = UpdateETHData();
        }

        private async Task UpdateETHData()
        {
            var result = await _ethService.GetDataAsync();
            await dispatcher.InvokeAsync(() =>
            {
                if(userControls.ContainsKey(RegionsName.EthPrice))
                    (userControls[RegionsName.EthPrice] as EthPriceUC).labelEthPrice.Content = $"${result.EthPrice.Result.Ethusd} ❙ {result.EthPrice.Result.Ethbtc} BTC";
                if (userControls.ContainsKey(RegionsName.GasTracker))
                {
                    (userControls[RegionsName.GasTracker] as GasTrackerUC).labelGasLow.Content = $"{result.EthGasPrice.Result.SafeGasPrice} gwei";
                    (userControls[RegionsName.GasTracker] as GasTrackerUC).labelGasAvg.Content = $"{result.EthGasPrice.Result.ProposeGasPrice} gwei";
                    (userControls[RegionsName.GasTracker] as GasTrackerUC).labelGasHigh.Content = $"{result.EthGasPrice.Result.FastGasPrice} gwei";
                }
                if (userControls.ContainsKey(RegionsName.BlockRewards))
                    (userControls[RegionsName.BlockRewards] as BlockRewardUC).labelBlockReward.Content = $"{result.AvgBlockReward.ToString().Replace(",", ".")} ETH";
                if (!string.IsNullOrEmpty(AppSettings.ethWallet) && userControls.ContainsKey(RegionsName.EthWallet))
                    (userControls[RegionsName.EthWallet] as EthWalletBalanceUC).labelWalletBalance.Content = $"{result.WalletBalance.ToString().Replace(",", ".")} ETH ❙ ${Math.Round(double.Parse(result.EthPrice.Result.Ethusd.Replace('.', ',')) * result.WalletBalance, 2).ToString().Replace(',', '.')}";
            });
        }


        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IETHService, ETHService>();
        }

        public static class RegionsName
        {
            public const string BlockRewards = "blockRewardsRegion";

            public const string EthPrice = "EthPriceRegion";

            public const string EthWallet = "EthWalletRegion";

            public const string GasTracker = "GasTrackerRegion";
        }
    }
}
