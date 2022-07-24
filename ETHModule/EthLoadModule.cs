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
using System.Windows.Controls;

namespace ETHModule
{
    public class EthLoadModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly AppSettings _appSettings;
        private SettingsMenu _settingsMenu;
        private readonly IRegionService _regionService;
        private IContainerProvider _containerProvider;
        private IETHService _ethService;
        private readonly ILogger<EthLoadModule> _logger;

        private int _regionsRequestedCount;
        private Dictionary<string, Func<UserControl>> _userControls;


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

            _userControls = new Dictionary<string, Func<UserControl>>()
            {
                { RegionsName.BlockRewards, () => _containerProvider.Resolve<BlockRewardUC>() },
                { RegionsName.EthPrice, () => _containerProvider.Resolve<EthPriceUC>() },
                { RegionsName.EthWallet, () => _containerProvider.Resolve<EthWalletBalanceUC>() },
                { RegionsName.GasTracker, () => _containerProvider.Resolve<GasTrackerUC>() }
            };

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

            _settingsMenu = _appSettings.GetMenu(Constants.Menu.MenuKey);
        }

        private void RequestRegions()
        {
            _regionService.RegionCreated += Manager_RegionCreated;

            var regionToRequest = new List<string>();

            if (!string.IsNullOrEmpty(_settingsMenu.Get<string>(Constants.Parameters.Wallet)))
                regionToRequest.Add(RegionsName.EthWallet);
            if (!_settingsMenu.Get<bool>(Constants.Parameters.hidePrice))
                regionToRequest.Add(RegionsName.EthPrice);
            if (!_settingsMenu.Get<bool>(Constants.Parameters.hideBlockReward))
                regionToRequest.Add(RegionsName.BlockRewards);
            if (!_settingsMenu.Get<bool>(Constants.Parameters.hideGasTracker))
                regionToRequest.Add(RegionsName.GasTracker);

            _regionsRequestedCount = regionToRequest.Count;

            regionToRequest.ForEach(r => _regionService.RegionRequest(r));
        }

        private void Manager_RegionCreated(string regName)
        {
            if (_userControls.ContainsKey(regName))
                AddRegion(regName, _userControls[regName].Invoke());
        }

        private void AddRegion(string name, UserControl userControl)
        {
            _regionManager.Regions[name].Add(userControl);
            if (--_regionsRequestedCount <= 0)
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
                var apiKey = _settingsMenu.Get<string>(Constants.Parameters.ApiKey);
                var updateTime = TimeSpan.FromMinutes(_settingsMenu.Get<int>(Constants.Parameters.UpdateTime));
                var wallet = _settingsMenu.Get<string>(Constants.Parameters.Wallet);
                var hidePrice = _settingsMenu.Get<bool>(Constants.Parameters.hidePrice) && string.IsNullOrEmpty(wallet);
                var hideGasTracker = _settingsMenu.Get<bool>(Constants.Parameters.hideGasTracker);
                var hideBlockReward = _settingsMenu.Get<bool>(Constants.Parameters.hideBlockReward);

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
