using prbd_2324_a06.Model;
using System.Collections.ObjectModel;
using System.Configuration;

namespace prbd_2324_a06.ViewModel;

public class MainViewModel : ViewModelCommon
{
    public string Title => "prbd_2324_a06";
    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users {
        get => _users;
        set => SetProperty(ref _users, value, () =>
        Console.WriteLine("cette ligne est appelé a l'assignatin du members"));
    }

    public MainViewModel() : base() {
        Users = new ObservableCollection<User>(Context.Users);
    }
}