using Test2Module.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using ModularWidget;

namespace Test2Module
{
    public class InitTestModule : IModule
    {
        string regName = "testregion";
        IRegionManager regionManager;
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
                regionManager.RegisterViewWithRegion(regName, typeof(ViewB));
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
