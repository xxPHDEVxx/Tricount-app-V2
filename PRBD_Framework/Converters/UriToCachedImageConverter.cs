using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace PRBD_Framework; 

// see: http://www.thejoyofcode.com/WPF_Image_element_locks_my_local_file.aspx
public class UriToCachedImageConverter : MarkupExtension, IValueConverter {
    private static List<string> _cache = new List<string>();

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        if (value == null || value is not string)
            return null;
        var path = Path.Combine(ApplicationRoot.IMAGE_PATH, value.ToString());
        if (!File.Exists(path))
            return null;
        if (!string.IsNullOrEmpty(path)) {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(path);
            bi.CacheOption = BitmapCacheOption.OnLoad;
            //Console.WriteLine(path + " :  " + _cache.Contains(path));
            if (!_cache.Contains(path)) {
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                _cache.Add(path);
            }
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException("Two way conversion is not supported.");
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return this;
    }

    public static void ClearCache() {
        _cache.Clear();
    }

    public static void ClearCache(string newPath) {
        _cache.Remove(newPath);
    }
}