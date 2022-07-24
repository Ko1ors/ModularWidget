using ETCModule.Models;
using ETCModule.Services;
using ETCModule.Settings;
using ETCModule.ViewModels;
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
        private const string PriceRegionName = "etcpriceregion";
        private const string WalletBalanceRegionName = "etcwalletregion";

        private readonly AppSettings _appSettings;
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;
        private IEtcService _etcService;
        private SettingsMenu _settingsMenu;
        private EtcPriceUC _priceUC;
        private EtcWalletBalanceUC _balanceUC;
        private int _regionsToCreate;


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
            _priceUC = new EtcPriceUC(new EtcPriceViewModel(_etcService));
            _balanceUC = new EtcWalletBalanceUC(new EtcWalletBalanceViewModel(_etcService));
            _regionsToCreate = 1;
            _regionService.RegionCreated += Manager_RegionCreated;

            if (!string.IsNullOrWhiteSpace(_settingsMenu.Get<string>(Constants.Parameters.Wallet)))
            {
                _regionsToCreate++;
                _regionService.RegionRequest(WalletBalanceRegionName);
            }
            _regionService.RegionRequest(PriceRegionName);
            _etcService.StartAsync();
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

            _settingsMenu = _appSettings.GetMenu(Constants.Menu.MenuKey);
        }

        private void Manager_RegionCreated(string regName)
        {
            if (regName == PriceRegionName)
            {
                _regionManager.Regions[regName].Add(_priceUC);
            }
            else if (regName == WalletBalanceRegionName)
            {
                _regionManager.Regions[regName].Add(_balanceUC);
            }
            else
                return;
            if (--_regionsToCreate < 1)
                _regionService.RegionCreated -= Manager_RegionCreated;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IEtcClientService, EtcClientService>();
            containerRegistry.RegisterSingleton<IEtcService, EtcService>();
        }
    }
}
