using ETCModule.Models;
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
    public class ETCLoadModule : IModule
    {
        private readonly AppSettings _appSettings;
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;

        private readonly string regName = "etcregion";
        private MainUC etcView = new MainUC();
        private EtcInformation etcInfo;

    

        public ETCLoadModule(AppSettings appSettings, IRegionManager regionManager, IRegionService regionService)
        {
            _appSettings = appSettings;
            _regionManager = regionManager;
            _regionService = regionService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            InitSettings();

            _regionService.RegionCreated += Manager_RegionCreated;
            _regionService.RegionRequest(regName);
            etcInfo = new EtcInformation(_appSettings);
            etcInfo.Completed += UpdateView;

            etcInfo.Start();
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

        private void UpdateView()
        {
            etcView.Dispatcher.Invoke(() =>
            {
                etcView.etcPriceUC.labelEtcPrice.Content = $"${etcInfo.lastEtcPrice.Result.CoinUsd} ❙ {Math.Round(etcInfo.lastEtcPrice.Result.CoinBtc, 5).ToString().Replace(",", ".")} BTC";
                if (!String.IsNullOrEmpty(etcInfo.etcWalletAddress))
                    etcView.etcWalletBalanceUC.labelEtcWalletBalance.Content = $"{etcInfo.lastWalletBalance.Replace(",", ".")} ETC ❙ ${Math.Round(Double.Parse(etcInfo.lastWalletBalance) * etcInfo.lastEtcPrice.Result.CoinUsd, 2).ToString().Replace(",", ".")}";
            });
        }

        private void Manager_RegionCreated(string regName)
        {
            if (regName == this.regName)
            {
                _regionService.RegionCreated -= Manager_RegionCreated;
                //regionManager.RegisterViewWithRegion(regName, typeof(MainUC));
                _regionManager.Regions[regName].Add(etcView);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
