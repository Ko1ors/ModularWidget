using Test2Module.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Test2Module
{
    public class InitTestModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion2", typeof(ViewB));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
