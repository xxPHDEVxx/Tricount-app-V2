using Microsoft.EntityFrameworkCore;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Globalization;

namespace prbd_2324_a06;

public partial class App
{
    public enum Messages
    {
        MSG_SIGN_UP,
        MSG_DISPLAY_SIGN_UP,
        MSG_LOGIN,
        MSG_LOGOUT,
        MSG_RESET,
        MSG_ADD_OPERATION,
        MSG_EDIT_OPERATION,
        MSG_DELETE_OPERATION,
        MSG_CLOSE_WINDOW,
        MSG_NEW_TRICOUNT,
        MSG_DISPLAY_TRICOUNT,
        MSG_DISPLAY_OPERATIONS,
        MSG_OPEN_NEW_OPERATION,
        MSG_OPEN_OPERATION
    }

    public App() {
        var ci = new CultureInfo("fr-BE") {
            DateTimeFormat = {
                ShortDatePattern = "dd/MM/yyyy",
                DateSeparator = "/"
            }
        };
        CultureInfo.DefaultThreadCurrentCulture = ci;
        CultureInfo.DefaultThreadCurrentUICulture = ci;
        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
    }

    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        TestQueries();

        // Login
        Register<User>(this, App.Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, PridContext>();
        });
        // Sign up
        Register<User>(this, App.Messages.MSG_SIGN_UP, user => {
            WindowCollection windowCollection = this.Windows;
            Login(user);

            NavigateTo<MainViewModel, User, PridContext>();
            // fermeture view sign up
            windowCollection[0]?.Close();
        });

        // Logout
        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, PridContext>();
        });

        // Reset
        Register(this, Messages.MSG_RESET, Reset);
    }
    public void Reset() {
        // Detached Entities from tracking
        Context.ChangeTracker.Clear();
        Context.SaveChanges();
        // Clear database and seed data
        PrepareDatabase();
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(1);
        Console.WriteLine("done");
    }

    protected override void OnRefreshData() {
        // TODO
    }

    private static void TestQueries() {
        // Un endroit pour tester vos requêtes LINQ
    }
}