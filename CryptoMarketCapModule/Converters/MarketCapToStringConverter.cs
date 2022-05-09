using System;
using System.Globalization;
using System.Windows.Data;

namespace CryptoMarketCapModule.Converters
{
    class MarketCapToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = (long)value;

            if (num >= 1000000000000)
                return (num / 1000000000000D).ToString("0.##") + " T";
            if (num >= 1000000000)
                return (num / 1000000000D).ToString("0.##") + " B";
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##") + " M";
            if (num >= 1000)
                return (num / 1000D).ToString("0.##") + " K";

            return num.ToString("#,0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
