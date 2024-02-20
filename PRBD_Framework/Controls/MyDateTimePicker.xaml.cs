using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PRBD_Framework; 

internal class TimeFormatConverter : IMultiValueConverter {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        //Console.WriteLine(string.Join("-", values));
        if (values.Length == 2 && values[0] != null && values[1] != null &&
            values[0] is TimeSpan time && values[1] is string format)
            return time.ToString(format);
        else
            return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        TimeSpan ts = default(TimeSpan);
        if (TimeSpan.TryParse((string)value, out ts)) {
            return new object[] { ts, null };
        } else return new object[] { ts, null };
    }
}

public partial class MyDateTimePicker : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    public void RaisePropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MyDateTimePicker() {
        InitializeComponent();
    }

    public DateTime? SelectedDateTime {
        get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
        set { SetValue(SelectedDateTimeProperty, value); }
    }

    public static readonly DependencyProperty SelectedDateTimeProperty =
        DependencyProperty.Register(
            "SelectedDateTime",
            typeof(DateTime?),
            typeof(MyDateTimePicker),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedDateTimeChanged));

    private static void SelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        //Console.WriteLine(e.Property + " - " + e.OldValue + " - " + e.NewValue);
        if (d is MyDateTimePicker picker)
            if (e.NewValue is DateTime newValue) {
                picker.SelectedDate = newValue.Date;
                picker.SelectedTime = newValue.TimeOfDay;
            } else {
                picker.SelectedDate = null;
                picker.SelectedTime = null;
            }
    }

    public DateTime? SelectedDate {
        get { return (DateTime)GetValue(SelectedDateProperty); }
        set { SetValue(SelectedDateProperty, value); }
    }

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register("SelectedDate",
            typeof(DateTime?),
            typeof(MyDateTimePicker),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedDateChanged));

    private static void SelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        //Console.WriteLine(e.Property + " - " + e.OldValue + " - " + e.NewValue);
        if (d is MyDateTimePicker picker)
            if (e.NewValue is DateTime newValue)
                picker.SelectedDateTime = newValue + (picker.SelectedDateTime?.TimeOfDay ?? TimeSpan.Zero);
            else
                picker.SelectedDateTime = null;
    }

    public TimeSpan? SelectedTime {
        get { return (TimeSpan)GetValue(SelectedTimeProperty); }
        set { SetValue(SelectedTimeProperty, value); }
    }

    public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register("SelectedTime",
            typeof(TimeSpan?),
            typeof(MyDateTimePicker),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedTimeChanged, CoerceTime));

    private static void SelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        //Console.WriteLine(e.Property + " - " + e.OldValue + " - " + e.NewValue);
        if (d is MyDateTimePicker picker)
            if (e.NewValue is TimeSpan newValue) {
                TimeSpan? newVal = (TimeSpan?)CoerceTime(d, newValue);
                picker.SelectedDateTime = picker.SelectedDateTime?.Date + newVal;
            } else
                picker.SelectedDateTime = null;
    }

    private static object CoerceTime(DependencyObject d, object value) {
        if (!(value is TimeSpan)) return null;
        TimeSpan time = (TimeSpan)value;
        if (time.Days > 0)
            time = new TimeSpan(0, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        //Console.WriteLine("coerced: " + time);
        return time;
    }

    public string TimeFormat {
        get { return (string)GetValue(TimeFormatProperty); }
        set { SetValue(TimeFormatProperty, value); }
    }

    public static readonly DependencyProperty TimeFormatProperty =
        DependencyProperty.Register(
            "TimeFormat",
            typeof(string),
            typeof(MyDateTimePicker),
            new FrameworkPropertyMetadata("hh\\:mm", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

    private void TextBox_GotFocus(object sender, RoutedEventArgs e) {
        textBox.SelectAll();
    }
}