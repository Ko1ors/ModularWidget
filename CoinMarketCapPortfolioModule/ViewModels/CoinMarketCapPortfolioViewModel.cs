using CoinMarketCapPortfolioModule.Models;
using Microsoft.Extensions.Logging;

namespace CoinMarketCapPortfolioModule.ViewModels
{
    public class CoinMarketCapPortfolioViewModel : ModelBase
    {
        private readonly ILogger<CoinMarketCapPortfolioViewModel> _logger;

        public PortfolioModel Portfolio { get; set; }

        public CoinMarketCapPortfolioViewModel()
        {
           Portfolio = new PortfolioModel()
           {
                Name = "Bitcoin",
                Price = 80000m,
                ChangePercent = -2.5m
           };
        }
    }
}
