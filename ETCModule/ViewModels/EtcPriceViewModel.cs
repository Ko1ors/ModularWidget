using ETCModule.Models;
using ETCModule.Services;
using System;

namespace ETCModule.ViewModels
{
    public class EtcPriceViewModel : ViewModelBase
    {
        private readonly IEtcService _etcService;
        private EtcPrice _price;

        public EtcPrice Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(PriceText));
            }
        }

        public string PriceText
        {
            get
            {
                if (Price is null)
                    return "$ーー ❙ ーー BTC";

                string priceText;
                if (Price.CoinUsd.HasValue && Price.CoinUsd.Value >= 0)
                    priceText = $"${Price.CoinUsd} ❙";
                else
                    priceText = "$ーー ❙";
                
                if (Price.CoinBtc.HasValue && Price.CoinBtc.Value >= 0)
                    priceText += $" {Math.Round(Price.CoinBtc.Value, 5).ToString().Replace(",", ".")} BTC";
                else
                    priceText += " ーー BTC";
                return priceText;
            }
        }

        public EtcPriceViewModel(IEtcService etcService)
        {
            _etcService = etcService;
            _etcService.EtcPriceUpdated += r => Price = r.Price;
        }

    }
}
