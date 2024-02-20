namespace PRBD_Framework; 

public abstract class DialogViewModelBase<U, C> : ViewModelBase<U, C>, IDialogViewModelBase
    where U : EntityBase<C> where C : DbContextBase, new() {

    private object _dialogResult;
    public object DialogResult {
        get => _dialogResult;
        set {
            _dialogResult = value;
            DoClose?.Invoke();
        }
    }

    public event Action DoClose;
}