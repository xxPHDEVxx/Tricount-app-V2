using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PRBD_Framework; 

public abstract class ObservableBase : ValidatableObjectBase, INotifyPropertyChanged, IDisposable {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _disposed;
    private List<string> _computedProps = new();

    protected ObservableBase() {
        // pour les propriétés calculées
        PropertyChanged += (_, e) => {
            if (_computedProps.Contains(e.PropertyName)) return;
            foreach (var prop in _computedProps)
                RaisePropertyChanged(prop);
        };
    }

    protected void AddComputedProperties(params string[] props) {
        _computedProps.AddRange(props);
    }

    public void RemoveComputedProperties(params string[] props) {
        _computedProps = _computedProps.Where(p => !props.Contains(p)).ToList();
    }

    public void ClearComputedProperties() {
        _computedProps.Clear();
    }

    protected void RaisePropertyChanged(string propertyName) {
        // Console.WriteLine($"RaisePropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RaisePropertyChanged(INotifyPropertyChanged source, string propertyName) {
        PropertyChanged?.Invoke(source, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Checks if a property already matches a desired value. Sets the property and
    /// notifies listeners only when necessary. (origin: Prism)
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="storage">Reference to a property with both getter and setter.</param>
    /// <param name="value">Desired value for the property.</param>
    /// <param name="model"></param>
    /// <param name="propertyName">Name of the property used to notify listeners. This
    ///     value is optional and can be provided automatically when invoked from compilers that
    ///     support CallerMemberName.</param>
    /// <returns>True if the value was changed, false if the existing value matched the
    /// desired value.</returns>
    protected virtual bool SetProperty<T>(ref T storage, T value,
        [CallerMemberName] string propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        //Validate();
        RaisePropertyChanged(propertyName);

        return true;
    }

    protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback,
        [CallerMemberName] string propertyName = null)
        where TModel : class {
        if (EqualityComparer<T>.Default.Equals(oldValue, newValue)) {
            return false;
        }

        callback(model, newValue);
        //Validate();
        RaisePropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Checks if a property already matches a desired value. Sets the property and
    /// notifies listeners only when necessary. (origin: Prism)
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="storage">Reference to a property with both getter and setter.</param>
    /// <param name="value">Desired value for the property.</param>
    /// <param name="propertyName">Name of the property used to notify listeners. This
    /// value is optional and can be provided automatically when invoked from compilers that
    /// support CallerMemberName.</param>
    /// <param name="onChanged">Action that is called after the property value has been changed.</param>
    /// <returns>True if the value was changed, false if the existing value matched the
    /// desired value.</returns>
    protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged,
        [CallerMemberName] string propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        onChanged?.Invoke();
        //Validate();
        RaisePropertyChanged(propertyName);

        return true;
    }

    protected void RaisePropertyChanged(params string[] propertyNames) {
        foreach (var n in propertyNames)
            RaisePropertyChanged(n);
    }

    public void RaisePropertyChanged(INotifyPropertyChanged source, params string[] propertyNames) {
        foreach (var n in propertyNames)
            RaisePropertyChanged(source, n);
    }

    /// <summary>
    /// Déclenche le PropertyChanged sur toutes les propriétés publiques.
    /// </summary>
    public void RaisePropertyChanged() {
        var type = GetType();
        foreach (var n in type.GetProperties())
            RaisePropertyChanged(n.Name);
    }

    public void RaisePropertyChanged(INotifyPropertyChanged source) {
        var type = GetType();
        foreach (var n in type.GetProperties())
            RaisePropertyChanged(source, n.Name);
    }

    //TODO: vérifier si bien implémenté : https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    public virtual void Dispose() {
        if (_disposed) return;
        //Console.WriteLine("Disposing " + this);
        ApplicationRoot.UnRegister(this);

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    protected void Register(Enum message, Action callback) {
        ApplicationRoot.Register(this, message, callback);
    }

    protected void Register<T>(Enum message, Action<T> callback) {
        ApplicationRoot.Register(this, message, callback);
    }

    protected static void NotifyColleagues(Enum message, object parameter) {
        ApplicationRoot.NotifyColleagues(message, parameter);
    }

    protected static void NotifyColleagues(Enum message) {
        ApplicationRoot.NotifyColleagues(message);
    }

    protected void UnRegister() {
        ApplicationRoot.UnRegister(this);
    }
}