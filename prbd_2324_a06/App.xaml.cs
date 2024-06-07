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
        MSG_NEW_TRICOUNT,
        MSG_DISPLAY_TRICOUNT,
        MSG_DISPLAY_OPERATIONS,
        MSG_OPEN_NEW_OPERATION,
        MSG_OPEN_OPERATION,
        MSG_OPERATION_CHANGED,
        MSG_TRICOUNT_CHANGED,
        MSG_CLOSE_TAB,
        MSG_PARTICIPANT_ADDED, 
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
        
        Register<Operation>(this, App.Messages.MSG_OPEN_NEW_OPERATION, operation => {
            DisableWindows();
        });
        Register<Operation>(this, App.Messages.MSG_OPEN_OPERATION, operation => {
            DisableWindows();
        });
        Register<Operation>(this, App.Messages.MSG_CLOSE_OPERATION_WINDOW, operation => {
            EnableWindows();
        });
        
        // Logout
        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, PridContext>();
        });

        // Reset
        Register(this, Messages.MSG_RESET, Reset);
    }
    
    // Bloquer fenêtres
    private void DisableWindows() {
        for (int i = 0; i < Windows.Count - 1; i++) {
            Windows[i]!.IsEnabled = false;
        }
    }
    // Débloquer fenêtres
    public void EnableWindows() {
        for (int i = 0; i < Windows.Count; i++) {
            Windows[i]!.IsEnabled = true;
        }
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