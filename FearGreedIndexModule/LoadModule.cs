using FearGreedIndexModule.Views;
using ModularWidget;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows.Controls;


namespace FearGreedIndexModule
{
    public class LoadModule : IModule
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
