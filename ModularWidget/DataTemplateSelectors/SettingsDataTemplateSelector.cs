using ModularWidget.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModularWidget.DataTemplateSelectors
{
    public class SettingsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate BoolTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var data = item as SettingsParameter;
            if (data.DataTypeName == "System.Boolean")
                return BoolTemplate;
            return DefaultTemplate;
            //switch (data.Enum)
            //{
            //    case Enum.Value1:
            //        return Template1;
            //    case Enum.Value2:
            //        return Template2;
            //    case Enum.Value3:
            //        return Template3;
            //    case Enum.Value4:
            //        return Template4;
            //    default:
            //        return null;
            //}
        }

    }
}
