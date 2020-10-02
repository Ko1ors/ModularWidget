using ModularWidget;
using MusicWinModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows.Controls;

namespace MusicWinModule
{
    public class MusicLoadModule : IModule
    {
        IRegionManager regionManager;
        private readonly string regName = "musicregion";
        private UserControl musicView = new MusicUC();
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
                //regionManager.RegisterViewWithRegion(regName, typeof(MainUC));
                regionManager.Regions[regName].Add(musicView);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
