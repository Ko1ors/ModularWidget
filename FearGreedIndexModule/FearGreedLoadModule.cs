using FearGreedIndexModule.ViewModels;
using FearGreedIndexModule.Views;
using ModularWidget.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;


namespace FearGreedIndexModule
{
    public class FearGreedLoadModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;
        private FearGreedIndexUC _fearGreedIndexUC;

        private const string RegionName = "feargreedindexregion";

        public FearGreedLoadModule(IRegionManager regionManager, IRegionService regionService)
        {
            _regionManager = regionManager;
            _regionService = regionService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _fearGreedIndexUC = containerProvider.Resolve<FearGreedIndexUC>();
            _regionService.RegionCreated += Manager_RegionCreated;
            _regionService.RegionRequest(RegionName);
        }

        private void Manager_RegionCreated(string regName)
        {
            if (regName == RegionName)
            {
                _regionService.RegionCreated -= Manager_RegionCreated;
                _regionManager.Regions[regName].Add(_fearGreedIndexUC);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<FearGreedIndexViewModel>();
            containerRegistry.Register<FearGreedIndexUC>();
        }
    }
}
