using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace PRBD_Framework; 

[ValueConversion(typeof(ReadOnlyObservableCollection<ValidationError>), typeof(string))]
public class ValidationErrorsToStringConverter : MarkupExtension, IValueConverter {
    public override object ProvideValue(IServiceProvider serviceProvider) {
        return new ValidationErrorsToStringConverter();
    }

    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) {
        ReadOnlyObservableCollection<ValidationError> errors =
            value as ReadOnlyObservableCollection<ValidationError>;

        if (errors == null) {
            return string.Empty;
        }

        if (errors.Count == 1)
            return errors.ElementAt(0).ErrorContent.ToString();

        return string.Join("\n", (from e in errors
            select "\u25cf " + e.ErrorContent.ToString()).ToArray());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}