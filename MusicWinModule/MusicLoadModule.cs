using ModularWidget.Services;
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

        private readonly string regName = "musicregion";
        private UserControl musicView = new MusicUC();

        public MusicLoadModule(IRegionManager regionManager, IRegionService regionService)
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
                _regionManager.Regions[regName].Add(musicView);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
