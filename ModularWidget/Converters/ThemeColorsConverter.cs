using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace ModularWidget.Converters
{
    public class ThemeColorsConverter : IMultiValueConverter
    {
        public static Color StringToColor(string colorStr)
        {
            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            var result = (Color)cc.ConvertFromString(colorStr);
            return result;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var colors = values[0] as IEnumerable<string>;

            var brush = new LinearGradientBrush();
            brush.StartPoint = new(0, 0);
            brush.EndPoint = new(1, 1);

            if (colors != null && colors.Count() >= 4)
            {
                brush.GradientStops.Add(
                    new GradientStop(StringToColor(colors.ToArray()[0]), 0.0));
                brush.GradientStops.Add(
                    new GradientStop(StringToColor(colors.ToArray()[1]), 0.25));
                brush.GradientStops.Add(
                    new GradientStop(StringToColor(colors.ToArray()[2]), 0.75));
                brush.GradientStops.Add(
                    new GradientStop(StringToColor(colors.ToArray()[3]), 1.0));
            }

            if (brush.GradientStops.Count < 4)
            {
                brush.GradientStops.Add(new GradientStop(Colors.Transparent, brush.GradientStops.Count));
            }

            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
