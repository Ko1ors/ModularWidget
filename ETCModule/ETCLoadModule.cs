using ModularWidget;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETCModule
{
    class ETCLoadModule : IModule
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
            //regionManager.RegisterViewWithRegion(regName, typeof(ViewB));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
