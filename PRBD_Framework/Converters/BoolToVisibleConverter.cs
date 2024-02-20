using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibleConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value != null && value is bool)
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}