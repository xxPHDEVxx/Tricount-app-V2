using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(Enum), typeof(Visibility))]
public class EnumToVisibleConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return value.Equals(true) ? parameter : Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}