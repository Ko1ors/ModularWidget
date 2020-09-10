using Test2Module.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using ModularWidget;

namespace Test2Module
{
    public class InitTestModule : IModule
    {
        IRegionManager regionManager;
        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager = containerProvider.Resolve<IRegionManager>();
            Manager.RegionCreated += Manager_RegionCreated;
            Manager.RegionRequest();
        }

        private void Manager_RegionCreated(string regName)
        {
            regionManager.RegisterViewWithRegion(regName, typeof(ViewB));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
