using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinMarketCapPortfolioModule.Models
{
    public class PortfolioModel : ModelBase
    {
        private string _name;
        private decimal _price;
        private decimal _changePercent;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
                OnPropertyChanged("PriceString");
            }
        }

        public string PriceString => $"$ {Price:F2}";

        public decimal ChangePercent
        {
            get { return _changePercent; }
            set
            {
                _changePercent = value;
                OnPropertyChanged("ChangePercent");
                OnPropertyChanged("ChangePercentString");
                OnPropertyChanged("ChangePercentColor");
                OnPropertyChanged("ChangePercentIcon");
            }
        }

        public string ChangePercentString => $"{Math.Abs(ChangePercent):P}";

        public bool ChangePercentPositive => ChangePercent > 0;

        public Brush ChangePercentColor => ChangePercentPositive ? (SolidColorBrush)new BrushConverter().ConvertFrom("#16c784") : (SolidColorBrush)new BrushConverter().ConvertFrom("#ea3943");

        public string ChangePercentIcon => ChangePercentPositive ? "CaretUp" : "CaretDown";
    }
}
