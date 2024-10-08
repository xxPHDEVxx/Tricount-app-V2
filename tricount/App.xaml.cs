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
        MSG_CLOSE_OPERATION_WINDOW,
        MSG_OPEN_TRICOUNT,
        MSG_EDIT_TRICOUNT,
        MSG_DISPLAY_TRICOUNT,
        MSG_DISPLAY_OPERATIONS,
        MSG_OPEN_NEW_OPERATION,
        MSG_OPEN_OPERATION,
        MSG_OPERATION_CHANGED,
        MSG_TRICOUNT_CHANGED,
        MSG_CLOSE_TAB,
        MSG_PARTICIPANT_ADDED,
        MSG_OPERATION_TRICOUNT_CHANGED,
        MSG_DELETED,
        MSG_TITLE_CHANGED,
        AMOUNT_CHANGED,
        MSG_REFRESH,
        SESSION1,
        SESSION1_DATA_CHANGED
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
            Login(user);

            NavigateTo<MainViewModel, User, PridContext>();
            // fermeture view sign up
            Windows[0]?.Close();
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
        // Clear database and seed data
        PrepareDatabase();
        NotifyColleagues(Messages.MSG_REFRESH);
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