using Microsoft.IdentityModel.Tokens;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class TricountDetailViewModel : ViewModelCommon
{
    // Commandes
    public ICommand AddCommand { get; set; }
    public ICommand AddMySelf { get; set; }
    public ICommand AddEvery { get; set; }
    public ICommand AddTemplate { get; set; }
    public ICommand SaveCommand { get; set; }
    public ICommand CancelCommand { get; set; }
    // Attributes et propriétés
    public ObservableCollectionFast<User> Users { get; } = new();
    private User _selectedUser;

    public User SelectedUser {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    private ObservableCollection<User> _participants;
    public ObservableCollection<User> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }
    private string _title;
    public string Title {
        get => _title;
        set => SetProperty(ref _title, value, ()=>Validate());
    }

    private string _description;
    public string Description {
        get => _description;
        set => SetProperty(ref _description, value, () => Validate());
    }

    private DateTime _date;
    public DateTime Date {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    private Tricount _tricount;
    public Tricount Tricount {
        get => _tricount;
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public TricountDetailViewModel(Tricount tricount, bool isNew) : base() {
        _tricount = tricount;
        _isNew = isNew;
        Users.RefreshFromModel(App.Context.Users.OrderBy(m => m.FullName));

        //SaveCommand = new RelayCommand(SaveAction, CanSaveAction);
        CancelCommand = new RelayCommand(CancelAction, CanCancelAction);
        AddCommand = new RelayCommand(AddParticipantAction, CanAddParticipantAction);
    }

    public override void SaveAction() {
        if (IsNew) {
            var tricount = new Tricount(Title, Description, Date, CurrentUser);
            Context.Tricounts.Add(Tricount);
            IsNew = false;
            foreach (var user in Participants) {
                Tricount.NewSubscriber(user.UserId);
            }
        }
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_TRICOUNT_CHANGED, Tricount);
    }

    private bool CanSaveAction() {
        if (IsNew)
            return Tricount.Validate() && !HasErrors;
        return Tricount != null && Tricount.IsModified && !HasErrors;
    }
    private void AddParticipantAction() {
        Console.WriteLine(SelectedUser);
        if (SelectedUser != null && !Participants.Contains(SelectedUser)) {
            Participants.Add(SelectedUser);
            NotifyColleagues(App.Messages.MSG_PARTICIPANT_ADDED, SelectedUser);
        }
    }

    private bool CanAddParticipantAction() {
        if (Participants == null) {
            return false;
           
        } else {
        return SelectedUser != null && !Participants.Contains(SelectedUser);
        }
    }
    public override void CancelAction() {
        ClearErrors();
        if (IsNew) {
            IsNew = false;
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
        } else {
           Tricount.Reload();
            RaisePropertyChanged();
        }
    }

   private bool CanCancelAction() {
      return Tricount != null && (IsNew || Tricount.IsModified);
    }

    public override bool Validate() {
        ClearErrors();
        // On délègue la validation à l'entité Tricount
        Tricount.Validate();
        // On ajoute les erreurs détectées par l'entité Tricount à notre propre liste d'erreurs
        AddErrors(Tricount.Errors);
        return !HasErrors;
    }



}
