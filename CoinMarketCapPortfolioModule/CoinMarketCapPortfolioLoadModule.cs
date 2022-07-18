using CoinMarketCapPortfolioModule.ViewModels;
using CoinMarketCapPortfolioModule.Views;
using ModularWidget;
using ModularWidget.Models;
using ModularWidget.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace CoinMarketCapPortfolioModule
{
    public class CoinMarketCapPortfolioLoadModule : IModule
    {
        private const string RegionName = "coinmarketcapportfolioregion";

        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;
        private readonly AppSettings _appSettings;

        private CoinMarketCapPortfolioUC _userControl;


        public CoinMarketCapPortfolioLoadModule(IRegionManager regionManager, IRegionService regionService, AppSettings appSettings)
        {
            _regionManager = regionManager;
            _regionService = regionService;
            _appSettings = appSettings;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _userControl = containerProvider.Resolve<CoinMarketCapPortfolioUC>();
            InitSettings();

            _regionService.RegionCreated += regionServiceRegionCreated;
            _regionService.RegionRequest(RegionName);
        }

        private void InitSettings()
        {
            var menu = new SettingsMenu(Constants.Menu.MenuKey, "CoinMarketCap Portfolio Settings");
            menu.Parameters.Add(new SettingsParameter<string>(Constants.Parameters.AuthTokern, "Auth Token", string.Empty));

            if (!_appSettings.MenuExists(menu.Key))
                _appSettings.AddOrUpdateMenu(menu);
            foreach (var parameter in menu.Parameters)
            {
                if (!_appSettings.ParameterExists(menu.Key, parameter.Key))
                    _appSettings.AddOrUpdateParameter(menu.Key, parameter);
            }
        }

        private void regionServiceRegionCreated(string regName)
        {
            if (regName == RegionName)
            {
                _regionService.RegionCreated -= regionServiceRegionCreated;
                _regionManager.Regions[regName].Add(_userControl);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<CoinMarketCapPortfolioViewModel>();
            containerRegistry.Register<CoinMarketCapPortfolioUC>();
        }
    }
}
