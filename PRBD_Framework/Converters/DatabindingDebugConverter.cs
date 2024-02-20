using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

public class DatabindingDebugConverter : MarkupExtension, IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        Debugger.Break();
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        Debugger.Break();
        return value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }
}