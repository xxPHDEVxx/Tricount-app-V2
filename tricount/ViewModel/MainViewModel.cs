using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows;
using System.Windows.Input;
namespace prbd_2324_a06.ViewModel;

public class MainViewModel : ViewModelCommon
{
    public ICommand ReloadDataCommand { get; set; }
    public ICommand Session1 { get; set; }

    public MainViewModel() : base() {
        ReloadDataCommand = new RelayCommand(() => {
            // refuser un reload s'il y a des changements en cours
            if (Context.ChangeTracker.HasChanges()) return;
            // permet de renouveller le contexte EF
            App.ClearContext();
            // notifie tout le monde qu'il faut rafraîchir les données
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });
        Session1 = new RelayCommand(() => NotifyColleagues(App.Messages.SESSION1));
    }

    public static string Title {
        get => $"MyTricount ({CurrentUser?.Mail})";
            }

    protected override void OnRefreshData() {
        // pour plus tard
    }
}