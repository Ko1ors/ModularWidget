using ModularWidget.Services;
using MusicWinModule.ViewModels;
using MusicWinModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Windows.Controls;

namespace MusicWinModule
{
    public class MusicLoadModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IRegionService _regionService;
        
        private const string RegionName = "musicregion";
        private UserControl _musicUC;

        public MusicLoadModule(IRegionManager regionManager, IRegionService regionService)
        {
            _regionManager = regionManager;
            _regionService = regionService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _musicUC = containerProvider.Resolve<MusicUC>();
            
            _regionService.RegionCreated += Manager_RegionCreated;
            _regionService.RegionRequest(RegionName);
        }

        private void Manager_RegionCreated(string regName)
        {
            if (regName == RegionName)
            {
                _regionService.RegionCreated -= Manager_RegionCreated;
                _regionManager.Regions[regName].Add(_musicUC);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MediaPlayerViewModel>();
            containerRegistry.Register<MusicUC>();
        }
    }
}
