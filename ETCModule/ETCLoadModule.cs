using ETCModule.Models;
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
        private MainUC etcView = new MainUC();
        private EtcInformation etcInfo;
        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager = containerProvider.Resolve<IRegionManager>();
            Manager.RegionCreated += Manager_RegionCreated;
            Manager.RegionRequest(regName);
            etcInfo = new EtcInformation();
            etcInfo.Completed += UpdateView;    
            etcInfo.Start();
        }

        private void UpdateView()
        {
            etcView.Dispatcher.Invoke(() =>
            {
                etcView.etcPriceUC.labelEtcPrice.Content = $"${etcInfo.lastEtcPrice.Result.Ethusd} ❙ {Math.Round(Double.Parse(etcInfo.lastEtcPrice.Result.Ethbtc),5)} BTC";
                if (!String.IsNullOrEmpty(Properties.Settings.Default.etcWalletAddress))
                    etcView.etcWalletBalanceUC.labelEtcWalletBalance.Content = $"{etcInfo.lastWalletBalance} ETC ❙ ${Double.Parse(etcInfo.lastWalletBalance) * Double.Parse(etcInfo.lastEtcPrice.Result.Ethusd)}";
            });
        }

        private void Manager_RegionCreated(string regName)
        {
            if (regName == this.regName)
            {
                Manager.RegionCreated -= Manager_RegionCreated;
                //regionManager.RegisterViewWithRegion(regName, typeof(MainUC));
                regionManager.Regions[regName].Add(etcView);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
