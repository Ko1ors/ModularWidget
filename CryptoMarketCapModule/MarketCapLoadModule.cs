using CryptoMarketCapModule.Views;
using ModularWidget;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace CryptoMarketCapModule
{
    public class MarketCapLoadModule : IModule
    {
        IRegionManager regionManager;
        private readonly string regName = "cryptomarketcapregion";
        private CryptoMarketCapUC cmcUC = new CryptoMarketCapUC();

        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager = containerProvider.Resolve<IRegionManager>();
            Manager.RegionCreated += Manager_RegionCreated;
            Manager.RegionRequest(regName);
        }


        private void Manager_RegionCreated(string regName)
        {
            if (regName == this.regName)
            {
                Manager.RegionCreated -= Manager_RegionCreated;
                regionManager.Regions[regName].Add(cmcUC);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {


        }
    }
}
