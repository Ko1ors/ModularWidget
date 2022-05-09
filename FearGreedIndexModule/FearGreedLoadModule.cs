using FearGreedIndexModule.Views;
using ModularWidget;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;


namespace FearGreedIndexModule
{
    public class FearGreedLoadModule : IModule
    {
        IRegionManager regionManager;
        private readonly string regName = "feargreedindexregion";

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
                regionManager.Regions[regName].Add(new FearGreedIndexUC());
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
