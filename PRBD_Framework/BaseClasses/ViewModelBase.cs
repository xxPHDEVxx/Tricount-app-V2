namespace PRBD_Framework; 

public abstract class ViewModelBase<U, C> : ObservableBase where U : EntityBase<C> where C : DbContextBase, new() {
    protected C Context => ApplicationRoot.Context<C>();

    protected ViewModelBase() {
        ApplicationRoot.Register(this, ApplicationBaseMessages.MSG_REFRESH_DATA, OnRefreshData);
    }

    protected virtual void OnRefreshData() { }

    public bool HasChanges => Context.ChangeTracker.HasChanges();

    public virtual void SaveAction() { }

    public virtual void CancelAction() { }

    public virtual bool MayLeave => !HasChanges;

    protected static U CurrentUser => ApplicationBase<U, C>.CurrentUser;

    protected static bool IsLoggedIn => ApplicationBase<U, C>.IsLoggedIn;

}