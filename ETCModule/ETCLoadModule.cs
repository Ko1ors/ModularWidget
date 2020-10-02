using ETCModule.Models;
using ETCModule.Views;
using ModularWidget;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;

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
                etcView.etcPriceUC.labelEtcPrice.Content = $"${etcInfo.lastEtcPrice.Result.Ethusd.Replace(",", ".")} ❙ {Math.Round(Double.Parse(etcInfo.lastEtcPrice.Result.Ethbtc), 5).ToString().Replace(",", ".")} BTC";
                if (!String.IsNullOrEmpty(etcInfo.etcWalletAddress))
                    etcView.etcWalletBalanceUC.labelEtcWalletBalance.Content = $"{etcInfo.lastWalletBalance.Replace(",", ".")} ETC ❙ ${Math.Round(Double.Parse(etcInfo.lastWalletBalance) * Double.Parse(etcInfo.lastEtcPrice.Result.Ethusd), 2).ToString().Replace(",", ".")}";
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
