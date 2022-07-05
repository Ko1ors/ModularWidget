using CoinMarketCapPortfolioModule.ViewModels;
using CoinMarketCapPortfolioModule.Views;
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
        
        private CoinMarketCapPortfolioUC _userControl;
        

        public CoinMarketCapPortfolioLoadModule(IRegionManager regionManager, IRegionService regionService)
        {
            _regionManager = regionManager;
            _regionService = regionService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _userControl = containerProvider.Resolve<CoinMarketCapPortfolioUC>();
            _regionService.RegionCreated += regionServiceRegionCreated;
            _regionService.RegionRequest(RegionName);
        }
        
        private void regionServiceRegionCreated(string regName)
        {
            if(regName == RegionName)
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
