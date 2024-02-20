using System.Windows.Controls;

namespace PRBD_Framework; 

public class UserControlBase : UserControl, IDisposable {
    private bool _disposed;

    public virtual void Dispose() {
        if (_disposed) return;
        //Console.WriteLine("Disposing " + this);
        ApplicationRoot.UnRegister(this);
        _disposed = true;
        (DataContext as IDisposable)?.Dispose();
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