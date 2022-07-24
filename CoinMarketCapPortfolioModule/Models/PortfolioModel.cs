using ModularWidget.Common.Models;
using System;
using System.Windows;
using System.Windows.Media;

namespace CoinMarketCapPortfolioModule.Models
{
    public class PortfolioModel : ModelBase
    {
        private string _name;
        private decimal _price;
        private decimal _changePercent;
        private decimal _changeBalance;
        private bool _privacyMode;

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

        public string PriceString => $"${Price:F2}";

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

        public decimal ChangeBalance
        {
            get { return _changeBalance; }
            set
            {
                _changeBalance = value;
                OnPropertyChanged("ChangeBalance");
            }
        }

        public bool PrivacyMode
        {
            get { return _privacyMode; }
            set
            {
                _privacyMode = value;
                OnPropertyChanged("PrivacyMode");
                OnPropertyChanged("BalanceVisibility");
                OnPropertyChanged("PrivacyBlocksVisibility"); ;
                OnPropertyChanged("PrivacyModeIcon");
            }
        }

        public string ChangeBalanceString => $"{ChangeBalanceSymbol} ${Math.Abs(ChangeBalance):F2}";

        public string ChangeBalanceSymbol => ChangeBalance >= 0 ? "+" : "-";

        public bool ChangePercentPositive => ChangePercent >= 0;

        public Brush ChangePercentColor => ChangePercentPositive ? (SolidColorBrush)new BrushConverter().ConvertFrom("#16c784") : (SolidColorBrush)new BrushConverter().ConvertFrom("#ea3943");

        public string ChangePercentIcon => ChangePercentPositive ? "CaretUp" : "CaretDown";

        public string PrivacyModeIcon => PrivacyMode ? "EyeSlash" : "Eye";

        public Visibility BalanceVisibility => PrivacyMode ? Visibility.Collapsed : Visibility.Visible;

        public Visibility PrivacyBlocksVisibility => PrivacyMode ? Visibility.Visible : Visibility.Collapsed;
    }
}
