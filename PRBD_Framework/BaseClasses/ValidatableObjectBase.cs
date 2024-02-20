using System.Collections;
using System.ComponentModel;

namespace PRBD_Framework; 

public abstract class ValidatableObjectBase : INotifyDataErrorInfo {
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    protected void AddError(string propertyName, string error) {
        if (!Errors.ContainsKey(propertyName)) {
            Errors[propertyName] = new List<string>();
        }

        Errors[propertyName].Add(error);
        RaiseErrorsChanged(propertyName);
    }

    protected void SetError(string propertyName, string error) {
        if (!Errors.ContainsKey(propertyName)) {
            Errors[propertyName] = new List<string>();
        }

        Errors[propertyName].Clear();
        Errors[propertyName].Add(error);
        RaiseErrorsChanged(propertyName);
    }

    protected void SetErrors(string propertyName, IEnumerable errors) {
        Errors[propertyName] = new List<string>();
        if (errors != null)
            foreach (var s in errors)
                Errors[propertyName].Add(s.ToString());
        RaiseErrorsChanged(propertyName);
    }

    protected void ClearErrors(string propertyName) {
        Errors[propertyName].Clear();
        RaiseErrorsChanged(propertyName);
    }

    protected void ClearErrors() {
        foreach (var key in Errors.Keys) {
            Errors[key].Clear();
            RaiseErrorsChanged(key);
        }
    }

    protected void RaiseErrors() {
        foreach (var key in Errors.Keys)
            RaiseErrorsChanged(key);
    }

    private void RaiseErrorsChanged(string propertyName) {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public IEnumerable GetErrors(string propertyName) {
        if (string.IsNullOrEmpty(propertyName) || !Errors.ContainsKey(propertyName))
            return null;
        return Errors[propertyName];
    }

    protected void AddErrors(Dictionary<string, ICollection<string>> errors) {
        foreach (var key in errors.Keys) {
            if (!Errors.ContainsKey(key))
                Errors[key] = new List<string>();

            foreach (var s in errors[key]) {
                Errors[key].Add(s);
                //TODO: bonne idée ?
                RaiseErrorsChanged(key);
            }
        }
    }

    protected void SetErrors(Dictionary<string, ICollection<string>> errors) {
        ClearErrors();
        AddErrors(errors);
    }

    public bool HasErrors {
        get { return Errors.Keys.Any(key => Errors[key].Count > 0); }
    }

    public Dictionary<string, ICollection<string>> Errors { get; } = new();

    public virtual bool Validate() => true;

    public virtual bool Validate(DbContextBase context) => true;
}