using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(string), typeof(bool))]
public class IsNegativeConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        double number;
        var isNumber = double.TryParse(value.ToString(), NumberStyles.Any, culture, out number);
        return isNumber && number < 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}