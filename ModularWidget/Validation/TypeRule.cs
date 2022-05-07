using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ModularWidget.Validation
{
    public class TypeRule : ValidationRule
    {
        public TypeWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var type = Type.GetType(Wrapper.TypeName);
            try
            {
                var t = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                return ValidationResult.ValidResult;
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Invalid type");
            }
        }
    }

    public class TypeWrapper : DependencyObject
    {
        public static readonly DependencyProperty TypeNameProperty =
             DependencyProperty.Register("TypeName", typeof(string),
             typeof(TypeWrapper), new FrameworkPropertyMetadata(""));

        public string TypeName
        {
            get { return (string)GetValue(TypeNameProperty); }
            set
            {
                SetValue(TypeNameProperty, value);
            }
        }
    }
}
