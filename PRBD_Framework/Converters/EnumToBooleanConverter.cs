using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumToBooleanConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return value.Equals(true) ? parameter : Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}