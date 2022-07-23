using ETHModule.Services;
using ETHModule.Settings;
using ETHModule.UserControls;
using Microsoft.Extensions.Logging;
using ModularWidget;
using ModularWidget.Models;
using ModularWidget.Services;
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
    public class EthLoadModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly AppSettings _appSettings;
        private readonly IRegionService _regionService;
        private IETHService _ethService;
        private readonly ILogger<EthLoadModule> _logger;

        private Timer timer;
        private int regionsRequestedCount;
        private Dictionary<string, UserControl> userControls;
        private Dispatcher dispatcher;


        public EthLoadModule(AppSettings appSettings, IRegionManager regionManager, IRegionService regionService, ILogger<EthLoadModule> logger)
        {
            _appSettings = appSettings;
            _regionManager = regionManager;
            _regionService = regionService;
            _logger = logger;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _ethService = containerProvider.Resolve<IETHService>();
            userControls = new Dictionary<string, UserControl>();
            dispatcher = Dispatcher.CurrentDispatcher;

            InitSettings();
            RequestRegions();
        }

        private void InitSettings()
        {
            var menu = new SettingsMenu(Constants.Menu.MenuKey, "Ethereum Module Settings");
            menu.Parameters.Add(new SettingsParameter<string>(Constants.Parameters.ApiKey, "Etherscan API Key", string.Empty));
            menu.Parameters.Add(new SettingsParameter<string>(Constants.Parameters.Wallet, "Ethereum Wallet", string.Empty));
            menu.Parameters.Add(new SettingsParameter<int>(Constants.Parameters.UpdateTime, "Update time (in minutes)", 5));
            menu.Parameters.Add(new SettingsParameter<bool>(Constants.Parameters.hidePrice, "Hide price", false));
            menu.Parameters.Add(new SettingsParameter<bool>(Constants.Parameters.hideBlockReward, "Hide block reward", false));
            menu.Parameters.Add(new SettingsParameter<bool>(Constants.Parameters.hideGasTracker, "Hide gas tracker", false));

            if (!_appSettings.MenuExists(menu.Key))
                _appSettings.AddOrUpdateMenu(menu);
            foreach (var parameter in menu.Parameters)
            {
                if (!_appSettings.ParameterExists(menu.Key, parameter.Key))
                    _appSettings.AddOrUpdateParameter(menu.Key, parameter);
            }
        }

        private void RequestRegions()
        {
            _regionService.RegionCreated += Manager_RegionCreated;

            regionsRequestedCount = 3;

            if (!string.IsNullOrEmpty(_appSettings.Get<string>(Constants.Parameters.Wallet)))
            {
                regionsRequestedCount++;
                _regionService.RegionRequest(RegionsName.EthWallet);
            }
            if (!_appSettings.Get<bool>(Constants.Parameters.hidePrice))
                _regionService.RegionRequest(RegionsName.EthPrice);
            if (!_appSettings.Get<bool>(Constants.Parameters.hideBlockReward))
                _regionService.RegionRequest(RegionsName.BlockRewards);
            if (!_appSettings.Get<bool>(Constants.Parameters.hideGasTracker))
                _regionService.RegionRequest(RegionsName.GasTracker);
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
            _regionManager.Regions[name].Add(userControl);
            if (--regionsRequestedCount <= 0)
            {
                _logger.LogInformation("All requested regions were created.");
                _regionService.RegionCreated -= Manager_RegionCreated;
                Task.Run(async () => await UpdateETHDataAsync());
            }
        }

        private async Task UpdateETHDataAsync()
        {
            try
            {
                var apiKey = _appSettings.Get<string>(Constants.Parameters.ApiKey);
                var updateTime = TimeSpan.FromMinutes(5);
                var wallet = _appSettings.Get<string>(Constants.Parameters.Wallet);
                var hidePrice = _appSettings.Get<bool>(Constants.Parameters.hidePrice) && string.IsNullOrEmpty(wallet);
                var hideGasTracker = _appSettings.Get<bool>(Constants.Parameters.hideGasTracker);
                var hideBlockReward = _appSettings.Get<bool>(Constants.Parameters.hideBlockReward);

                _ethService.EthPriceUpdated += EthServicePriceUpdated;
                _ethService.GasPriceUpdated += EthServiceGasTrackerUpdated;
                _ethService.AvgBlockRewardUpdated += EthServiceBlockRewardUpdated;
                _ethService.EthUpdated += EthServiceEthUpdated;

                await _ethService.StartAsync(apiKey, updateTime, wallet, hidePrice, hideGasTracker, hideBlockReward);

                _logger.LogInformation($"Starting ETH Service. API Key: {apiKey}, UpdateTime: {updateTime.TotalMinutes} minutes, Wallet: {wallet}, Hide price: {hidePrice}, Hide gas tracker: {hideGasTracker}, Hide block reward: {hideBlockReward}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating ETH data.");
            }
        }

        private void EthServiceEthUpdated(Data.ETHCompositeModel value)
        {
            dispatcher.InvokeAsync(() =>
            {
                if (userControls.ContainsKey(RegionsName.EthWallet))
                {
                    var priceString = value.EthPrice != null ? $"❙ ${Math.Round(double.Parse(value.EthPrice.Result.Ethusd.Replace('.', ',')) * value.WalletBalance, 2).ToString().Replace(',', '.')}" : "";
                    (userControls[RegionsName.EthWallet] as EthWalletBalanceUC).labelWalletBalance.Content = $"{value.WalletBalance.ToString().Replace(",", ".")} ETH " + priceString;
                }
            });
        }

        private void EthServiceBlockRewardUpdated(double value)
        {
            dispatcher.InvokeAsync(() =>
            {
                if (userControls.ContainsKey(RegionsName.BlockRewards))
                    (userControls[RegionsName.BlockRewards] as BlockRewardUC).labelBlockReward.Content = $"{value.ToString().Replace(",", ".")} ETH";
            });
        }

        private void EthServiceGasTrackerUpdated(Data.EthGasPrice value)
        {
            dispatcher.InvokeAsync(() =>
            {
                if (userControls.ContainsKey(RegionsName.GasTracker) && value != null)
                {
                    (userControls[RegionsName.GasTracker] as GasTrackerUC).labelGasLow.Content = $"{value.Result.SafeGasPrice} gwei";
                    (userControls[RegionsName.GasTracker] as GasTrackerUC).labelGasAvg.Content = $"{value.Result.ProposeGasPrice} gwei";
                    (userControls[RegionsName.GasTracker] as GasTrackerUC).labelGasHigh.Content = $"{value.Result.FastGasPrice} gwei";
                }
            });
        }

        private void EthServicePriceUpdated(Data.EthPrice value)
        {
            dispatcher.InvokeAsync(() =>
            {
                if (userControls.ContainsKey(RegionsName.EthPrice) && value != null)
                    (userControls[RegionsName.EthPrice] as EthPriceUC).labelEthPrice.Content = $"${value.Result.Ethusd} ❙ {value.Result.Ethbtc} BTC";
            });
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IETHService, EthService>();
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
