using ETHModule.Services;
using ETHModule.Settings;
using ETHModule.UserControls;
using ETHModule.ViewModels;
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
        private IContainerProvider _containerProvider;
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
            _containerProvider = containerProvider;
            _ethService = _containerProvider.Resolve<IETHService>();
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

            var regionToRequest = new List<string>();

            if (!string.IsNullOrEmpty(_appSettings.Get<string>(Constants.Parameters.Wallet)))
                regionToRequest.Add(RegionsName.EthWallet);
            if (!_appSettings.Get<bool>(Constants.Parameters.hidePrice))
                regionToRequest.Add(RegionsName.EthPrice);
            if (!_appSettings.Get<bool>(Constants.Parameters.hideBlockReward))
                regionToRequest.Add(RegionsName.BlockRewards);
            if (!_appSettings.Get<bool>(Constants.Parameters.hideGasTracker))
                regionToRequest.Add(RegionsName.GasTracker);

            regionsRequestedCount = regionToRequest.Count;

            regionToRequest.ForEach(r => _regionService.RegionRequest(r));
        }

        private void Manager_RegionCreated(string regName)
        {
            switch (regName)
            {
                case RegionsName.BlockRewards:
                    AddRegion(regName, _containerProvider.Resolve<BlockRewardUC>());
                    break;
                case RegionsName.EthPrice:
                    AddRegion(regName, _containerProvider.Resolve<EthPriceUC>());
                    break;
                case RegionsName.EthWallet:
                    AddRegion(regName, _containerProvider.Resolve<EthWalletBalanceUC>());
                    break;
                case RegionsName.GasTracker:
                    AddRegion(regName, _containerProvider.Resolve<GasTrackerUC>());
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
                _ = StartEthUpdateAsync();
            }
        }

        private async Task StartEthUpdateAsync()
        {
            try
            {
                var apiKey = _appSettings.Get<string>(Constants.Parameters.ApiKey);
                var updateTime = TimeSpan.FromMinutes(_appSettings.Get<int>(Constants.Parameters.UpdateTime));
                var wallet = _appSettings.Get<string>(Constants.Parameters.Wallet);
                var hidePrice = _appSettings.Get<bool>(Constants.Parameters.hidePrice) && string.IsNullOrEmpty(wallet);
                var hideGasTracker = _appSettings.Get<bool>(Constants.Parameters.hideGasTracker);
                var hideBlockReward = _appSettings.Get<bool>(Constants.Parameters.hideBlockReward);

                await _ethService.StartAsync(apiKey, updateTime, wallet, hidePrice, hideGasTracker, hideBlockReward);

                _logger.LogInformation($"Starting ETH Service. API Key: {apiKey}, UpdateTime: {updateTime.TotalMinutes} minutes, Wallet: {wallet}, Hide price: {hidePrice}, Hide gas tracker: {hideGasTracker}, Hide block reward: {hideBlockReward}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating ETH data.");
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IETHService, EthService>();
            
            containerRegistry.Register<EthPriceViewModel>();
            containerRegistry.Register<EthWalletBalanceViewModel>();
            containerRegistry.Register<GasTrackerViewModel>();
            containerRegistry.Register<BlockRewardViewModel>();
            
            containerRegistry.Register<EthPriceUC>();
            containerRegistry.Register<EthWalletBalanceUC>();
            containerRegistry.Register<GasTrackerUC>();
            containerRegistry.Register<BlockRewardUC>();
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
