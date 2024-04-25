using prbd_2324_a06.Model;
using System.Collections.ObjectModel;
using System.Configuration;

namespace prbd_2324_a06.ViewModel;

public class MainViewModel : PRBD_Framework.ViewModelBase<User, PridContext>
{
    public string Title => "prbd_2324_a06";
    private ObservableCollection<User> _members;
    public ObservableCollection<User> Members {
        get => _members;
        set => SetProperty(ref _members, value, () =>
        Console.WriteLine("cette ligne est appelé a l'assignatin du members"));
    }

    public MainViewModel() : base() {
        Members = new ObservableCollection<User>(Context.Users);
    }
}