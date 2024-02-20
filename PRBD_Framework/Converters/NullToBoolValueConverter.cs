using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(object), typeof(bool))]
public class NullToBoolValueConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        bool result = value != null;
        if (parameter != null)
            return !result;
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}