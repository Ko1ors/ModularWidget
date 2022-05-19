using ModularWidget.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Test2Module.Views;

namespace Test2Module
{
    public class InitTestModule : IModule
    {
        string regName = "testregion";
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;

        public InitTestModule(IRegionManager regionManager, IRegionService regionService)
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
                _regionManager.RegisterViewWithRegion(regName, typeof(ViewB));
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
