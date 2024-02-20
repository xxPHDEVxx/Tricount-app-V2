namespace PRBD_Framework; 

public abstract class ApplicationBase<U, C> : ApplicationRoot
    where U : EntityBase<C> where C : DbContextBase, new() {
    public static C Context => Context<C>();

    public static U CurrentUser { get; protected set; }

    protected static void Login(U user) {
        CurrentUser = user;
    }

    protected static void Logout() {
        CurrentUser = null;
    }

    public static bool IsLoggedIn => CurrentUser != null;
}