using System.ComponentModel;
using System.Windows;

namespace PRBD_Framework; 

public class WindowBase : Window, IDisposable {
    private bool _disposed;

    public virtual void Dispose() {
        if (_disposed) return;
        //Console.WriteLine("Disposing " + this);
        _disposed = true;
        (DataContext as IDisposable)?.Dispose();
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        Dispose();
    }

    public void Register(Enum message, Action callback) {
        ApplicationRoot.Register(this, message, callback);
    }

    public void Register<T>(Enum message, Action<T> callback) {
        ApplicationRoot.Register(this, message, callback);
    }

    public static void NotifyColleagues(Enum message, object parameter) {
        ApplicationRoot.NotifyColleagues(message, parameter);
    }

    public static void NotifyColleagues(Enum message) {
        ApplicationRoot.NotifyColleagues(message);
    }

    public void UnRegister() {
        ApplicationRoot.UnRegister(this);
    }
}