using ETCModule.Models;
using ETCModule.Services;
using ETCModule.Settings;
using ETCModule.Views;
using ModularWidget;
using ModularWidget.Models;
using ModularWidget.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;

namespace ETCModule
{
    public class EtcLoadModule : IModule
    {
        private readonly AppSettings _appSettings;
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;
        private IEtcService _etcService;

        private readonly string regName = "etcregion";
        private MainUC etcView = new MainUC();


        public EtcLoadModule(AppSettings appSettings, IRegionManager regionManager, IRegionService regionService)
        {
            _appSettings = appSettings;
            _regionManager = regionManager;
            _regionService = regionService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            InitSettings();
            _etcService = containerProvider.Resolve<IEtcService>();

            _regionService.RegionCreated += Manager_RegionCreated;
            _etcService.EtcUpdated += EtcService_EtcUpdated;

            _regionService.RegionRequest(regName);
            _etcService.Start();
        }

        private void EtcService_EtcUpdated(EtcCompositeResult result)
        {
            etcView.Dispatcher.Invoke(() =>
            {
                etcView.etcPriceUC.labelEtcPrice.Content = $"${result.EtcPrice.Result.CoinUsd} ❙ {Math.Round(result.EtcPrice.Result.CoinBtc, 5).ToString().Replace(",", ".")} BTC";
                if (result.WalletBalance >= 0)
                    etcView.etcWalletBalanceUC.labelEtcWalletBalance.Content = $"{result.WalletBalance.ToString().Replace(",", ".")} ETC ❙ ${Math.Round(result.WalletBalance * result.EtcPrice.Result.CoinUsd, 2).ToString().Replace(",", ".")}";
            });
        }

        private void InitSettings()
        {
            var menu = new SettingsMenu(Constants.Menu.MenuKey, "ETC Module Settings");
            menu.Parameters.Add(new SettingsParameter<string>(Constants.Parameters.Wallet, "ETC Wallet", string.Empty));
            menu.Parameters.Add(new SettingsParameter<int>(Constants.Parameters.UpdateTime, "Update time (in minutes)", 5));

            if (!_appSettings.MenuExists(menu.Key))
                _appSettings.AddOrUpdateMenu(menu);
            foreach (var parameter in menu.Parameters)
            {
                if (!_appSettings.ParameterExists(menu.Key, parameter.Key))
                    _appSettings.AddOrUpdateParameter(menu.Key, parameter);
            }
        }

        private void Manager_RegionCreated(string regName)
        {
            if (regName == this.regName)
            {
                _regionService.RegionCreated -= Manager_RegionCreated;
                _regionManager.Regions[regName].Add(etcView);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IEtcService, EtcService>();
        }
    }
}
