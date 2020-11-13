using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using ModularWidget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberpunkReleaseCountdownModule.Views;

namespace CyberpunkReleaseCountdownModule
{
    class CyberpunkLoadModule : IModule
    {
        IRegionManager regionManager;
        private readonly string regName = "cyberpunkregion";
        private CyberpunkUC cpUC = new CyberpunkUC();
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
                regionManager.Regions[regName].Add(cpUC);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
