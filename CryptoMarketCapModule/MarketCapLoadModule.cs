using CryptoMarketCapModule.Views;
using ModularWidget.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace CryptoMarketCapModule
{
    public class MarketCapLoadModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;

        private readonly string regName = "cryptomarketcapregion";
        private CryptoMarketCapUC cmcUC = new CryptoMarketCapUC();

        public MarketCapLoadModule(IRegionManager regionManager, IRegionService regionService)
        {
            _regionManager = regionManager;
            _regionService = regionService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionService.RegionCreated += Manager_RegionCreated;
            _regionService.RegionRequest(regName);
        }


        private void Manager_RegionCreated(string regName)
        {
            if (regName == this.regName)
            {
                _regionService.RegionCreated -= Manager_RegionCreated;
                _regionManager.Regions[regName].Add(cmcUC);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
