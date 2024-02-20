using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace PRBD_Framework;

public class ApplicationRoot : Application {
    private static readonly Messenger Messenger = new();
    private static readonly Dictionary<object, List<Tuple<Enum, Guid>>> Ids = new();

    public static string IMAGE_PATH {
        get {
            // vérifie d'abord si on trouve un dossier images à côté de l'exécutable
            var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                        "/images");
            if (Directory.Exists(path)) return path;

            // Si ce n'est pas le cas, on feinte en se basant sur le stack trace pour retrouver le path du code source du projet
            // C'est ce qui sera utilisé quand on est dans VS en mode design.
            // (voir https://stackoverflow.com/a/20999702)
            var trace = new StackTrace(true);
            foreach (var frame in trace.GetFrames()) {
                path = Path.GetFullPath(Path.GetDirectoryName(frame.GetFileName()) + "/images");
                if (Directory.Exists(path))
                    return path;
            }

            return null;
        }
    }

    public static string GetAbsolutePicturePath(string relativePath) {
        return Path.Combine(IMAGE_PATH, relativePath);
    }

    private static DbContextBase _context;

    public static TC Context<TC>() where TC : DbContextBase, new() {
        try {
            if (_context != null)
                return (TC)_context;

            _context = new TC();
            _context.SaveChangesFailed += ContextSaveChangesFailed;

            return (TC)_context;
        } catch (Exception) {
            return null;
        }
    }

    private static void ContextSaveChangesFailed(object sender, SaveChangesFailedEventArgs e) {
        string error = e.Exception.Message;
        if (e.Exception is DbUpdateConcurrencyException)
            error = "The data you're trying to save have been changed by someone else.\n\n" +
                    "They will be reverted now to the values stored in the database.";
        System.Windows.MessageBox.Show(error, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void ClearContext() {
        _context = null;
    }

    protected virtual void OnRefreshData() {
        //Console.WriteLine(GetType().FullName + "." + nameof(OnRefreshData));
    }

    protected ApplicationRoot() {
        Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        Register(this, ApplicationBaseMessages.MSG_REFRESH_DATA, OnRefreshData);
    }

    private void Dispatcher_UnhandledException(object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        if (e.Exception is DbUpdateException exception) {
            //Console.WriteLine(ex.InnerException.StackTrace);
            System.Windows.MessageBox.Show(exception.GetBaseException().Message, "Database Error! Shutting down...",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        } else {
            //Console.WriteLine(e.Exception.StackTrace);
            System.Windows.MessageBox.Show(e.Exception.ToString());
            e.Handled = true;
        }

        Shutdown();
    }

    private static Type MapViewModelToView(Type viewModelType) {
        if (viewModelType.FullName == null)
            return null;

        var viewTypeName = viewModelType.FullName.Replace("ViewModel", "View");
        return viewModelType.Assembly.GetType(viewTypeName, true);
    }

    protected static void NavigateTo<T, TU, TC>() where T : ViewModelBase<TU, TC>
        where TU : EntityBase<TC>
        where TC : DbContextBase, new() {
        var win = Current.MainWindow;
        Current.MainWindow = (WindowBase)Activator.CreateInstance(MapViewModelToView(typeof(T)));
        Current.MainWindow?.Show();
        win?.Close();
    }

    public static object ShowDialog<T, TU, TC>(params object[] args)
        where T : DialogViewModelBase<TU, TC> where TU : EntityBase<TC> where TC : DbContextBase, new() {
        var frm = (DialogWindowBase)Activator.CreateInstance(MapViewModelToView(typeof(T)), args);
        if (frm == null)
            return null;

        frm.ShowDialog();
        return frm.GetViewModel<TU, TC>()?.DialogResult;
    }

    protected static MessageBoxResult MessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage image, Window window = null) {
        return System.Windows.MessageBox.Show(window ?? Current?.MainWindow!, text, title, buttons, image);
    }

    public static void Error(string text, Window window = null) {
        MessageBox(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error, window);
    }

    public static void Warning(string text, Window window = null) {
        MessageBox(text, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, window);
    }

    public static bool Confirm(string text, Window window = null) {
        return MessageBox(text, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, window) == MessageBoxResult.Yes;
    }

    public static void ChangeCulture(string culture) {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        var oldWindow = Current.MainWindow;
        if (oldWindow == null)
            return;

        var type = oldWindow.GetType();
        Window newWindow = (Window)Activator.CreateInstance(type);
        if (newWindow != null) {
            newWindow.Show();
            Current.MainWindow = newWindow;
        }

        oldWindow.Close();
    }

    public static void Register(object owner, Enum message, Action callback) {
        var id = Messenger.Register(message, callback);
        if (!Ids.ContainsKey(owner))
            Ids[owner] = new List<Tuple<Enum, Guid>>();
        Ids[owner].Add(new Tuple<Enum, Guid>(message, id));
    }

    public static void Register<T>(object owner, Enum message, Action<T> callback) {
        var id = Messenger.Register(message, callback);
        if (!Ids.ContainsKey(owner))
            Ids[owner] = new List<Tuple<Enum, Guid>>();
        Ids[owner].Add(new Tuple<Enum, Guid>(message, id));
    }

    public static void NotifyColleagues(Enum message, object parameter) {
        Messenger.NotifyColleagues(message, parameter);
    }

    public static void NotifyColleagues(Enum message) {
        Messenger.NotifyColleagues(message);
    }

    public static void UnRegister(object owner) {
        if (!Ids.ContainsKey(owner))
            return;

        foreach (var tuple in Ids[owner])
            Messenger.UnRegister(tuple.Item1, tuple.Item2);
    }
}