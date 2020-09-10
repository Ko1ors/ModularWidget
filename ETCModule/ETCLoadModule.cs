using ETCModule.Views;
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
    public class ETCLoadModule : IModule
    {
        IRegionManager regionManager;
        private readonly string regName = "etcregion";
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
                regionManager.RegisterViewWithRegion(regName, typeof(EtcWalletBalanceUC));
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
