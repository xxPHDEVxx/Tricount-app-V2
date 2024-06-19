using Microsoft.IdentityModel.Tokens;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class CardParticipantViewModel : ViewModelCommon {
    public TricountDetailViewModel vm { get; set; }
    public User User { get; set; }
    public ICommand DeleteCommand { get; set; }
    private Visibility _visible;

    public Visibility Visible {
        get => _visible;
        set => SetProperty(ref _visible, value);
    }
    public CardParticipantViewModel(User user) {
        User = user;
        DeleteCommand = new RelayCommand(DeleteAction);
        Visible = User == App.CurrentUser ? Visibility.Hidden : Visibility.Visible;
    }

    private void DeleteAction() {
        Console.WriteLine("delete");
    }

    public string Current => App.CurrentUser == User ? "(creator)" : "";
    
}

