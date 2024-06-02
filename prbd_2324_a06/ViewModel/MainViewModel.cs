using PRBD_Framework;
using System.Windows.Input;
namespace prbd_2324_a06.ViewModel;

public class MainViewModel : ViewModelCommon
{
    public ICommand ReloadDataCommand { get; set; }

    public MainViewModel() : base() {
        ReloadDataCommand = new RelayCommand(() => {
            // refuser un reload s'il y a des changements en cours
            if (Context.ChangeTracker.HasChanges()) return;
            // permet de renouveller le contexte EF
            App.ClearContext();
            // notifie tout le monde qu'il faut rafraîchir les données
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        });
    }

    public static string Title {
        get => $"My Social Network ()";
    }

    protected override void OnRefreshData() {
        // pour plus tard
    }
}