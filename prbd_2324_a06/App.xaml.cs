using Castle.Components.DictionaryAdapter.Xml;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Globalization;
using System.Xml;

namespace prbd_2324_a06;

public partial class App
{
    public enum Messages
    {
        MSG_SIGN_UP,
        MSG_DISPLAY_SIGN_UP,
        MSG_LOGIN,
        MSG_LOGOUT
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