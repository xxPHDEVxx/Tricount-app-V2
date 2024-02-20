using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(int), typeof(bool))]
public class GreaterThanZeroToVisibleConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}