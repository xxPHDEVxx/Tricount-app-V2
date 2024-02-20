using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(string), typeof(bool))]
public class PositiveToVisibleConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        double number;
        var isNumber = double.TryParse(value.ToString(), NumberStyles.Any, culture, out number);
        return isNumber && number >= 0.0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}