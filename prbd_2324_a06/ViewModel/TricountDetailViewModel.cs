using Microsoft.IdentityModel.Tokens;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class TricountDetailViewModel : ViewModelCommon
{
    private User GetCurrentUser() {
        return App.CurrentUser;
    }

    //// Commandes
    public ICommand AddCommand { get; set; }
    public ICommand AddMySelf { get; set; }
    public ICommand AddEvery { get; set; }
    public ICommand AddTemplate { get; set; }
    public ICommand SaveCommand { get; set; }
    public ICommand CancelCommand { get; set; }
    public ICommand DeleteCommand { get; set; }

    // Attributes et propriétés
    public ObservableCollectionFast<User> Users { get; } = new ObservableCollectionFast<User>();

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
    public string CreatedBy => $"Created By {CurrentUser.FullName} on {DateTime.Now.ToString("dd/MM/yyyy")}";
    private string _title;

    public string Title {
        get => _title;
        set => SetProperty(ref _title, value, () => Validate());
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
        set => SetProperty(ref _tricount, value);
    }

    private User _user;

    public User User {
        get => _user;
        set => SetProperty(ref _user, value);
    }
    private string _current;

    public string Current {
        get => _current;
        set => SetProperty(ref _current, value);
    }

    private bool _isNew;

    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }


    public TricountDetailViewModel(Tricount tricount, bool isNew) : base() {
        Tricount = tricount;
        IsNew = isNew;

        Users.RefreshFromModel(Context.Users
            .Where(u => u.UserId != CurrentUser.UserId && u.Role == Role.User)
            .OrderBy(m => m.FullName)
        );

        Current = CurrentUser.FullName;

        if (!IsNew) {
            Tricount.Title = tricount.Title;
            Tricount.Description = tricount.Description;
            Tricount.CreatedAt = tricount.CreatedAt;
        }

        Participants = new ObservableCollection<User>();
        SaveCommand = new RelayCommand(SaveAction, CanSaveAction);
        CancelCommand = new RelayCommand(CancelAction, CanCancelAction);
        AddCommand = new RelayCommand(AddParticipantAction, CanAddParticipantAction);
        AddMySelf = new RelayCommand(AddMySelfAction,CanAddMySelfAction);
        AddEvery = new RelayCommand(AddAllAction, CanAddAllAction);

        DeleteCommand = new RelayCommand(DeleteParticipantAction);

        OnRefreshData();

    }

    private void DeleteParticipantAction() {
        Console.WriteLine("delete");
    }

    public string IsCurrentUser(User us) {
        return User == us ? " (creator)" : "";
    }

    protected override void OnRefreshData() {

        Participants.Add(CurrentUser);
    }
    public override void SaveAction() {
        // Add propriétés au tricount 
        if (IsNew) {
            Tricount = new Tricount(Title, Description, DateTime.Today, User);
            Context.Add(Tricount);
            AddSubscriptions();
        }

        // ajouter code pour edit
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_TRICOUNT_CHANGED, Tricount);
    }

    // Add Subscriptions
    public void AddSubscriptions() {
        if (Tricount != null) {
            if (IsNew) {
                Tricount.Subscriptions.Add(new Subscription(User.UserId, Tricount.Id));
            }

            foreach (var user in Participants) {
                Tricount.Subscriptions.Add(new Subscription(User.UserId, Tricount.Id));
            }
        }
    }

    // Validation before the save Action
    private bool CanSaveAction() {
        if (IsNew)
            return Validate() && !HasErrors;
        return Tricount != null && !HasErrors;
    }

    private void AddParticipantAction() {
        if (Participants != null && SelectedUser != null) {
            Participants.Add(SelectedUser);
            Users.Remove(SelectedUser);
            NotifyColleagues(App.Messages.MSG_PARTICIPANT_ADDED, SelectedUser);
            Console.WriteLine(Participants.Count);
        }

        if (Participants == null || SelectedUser == null || Participants.Contains(SelectedUser)) {
            return;
        }

        Participants.Add(SelectedUser);
        Users.Remove(SelectedUser);
        Console.WriteLine(Participants.Count);
        NotifyColleagues(App.Messages.MSG_PARTICIPANT_ADDED, SelectedUser);
        SelectedUser = null;
    }

    private bool CanAddParticipantAction() {
        if (!Users.IsNullOrEmpty() && SelectedUser != null) {
            return true;
        }
        return false;
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
        if (string.IsNullOrWhiteSpace(Title))
            AddError(nameof(Title), "required");
        else if (Title.Length < 3)
            AddError(nameof(Title), "length must be >= 3");


        // Validation de la description si elle contient quelque chose
        if (!string.IsNullOrWhiteSpace(Description) && Description.Length < 3) {
            AddError(nameof(Description), "length must be >= 3, ");
        }

        return !HasErrors;
    }

    private void AddMySelfAction() {
        if (!IsNew) {
            var currentUser = GetCurrentUser();
            if (!Participants.Contains(currentUser)) {
                Participants.Add(currentUser);
            }
        }
    }

    private bool CanAddMySelfAction() {
        var currentUser = GetCurrentUser();
        return (!Participants.Contains(currentUser) && !IsNew);
    }

    private void AddAllAction() {
        if (IsNew) {
            foreach (var user in Users) {
                if (!Participants.Contains(user)) {
                    Participants.Add(user);
                }
            }
            Users.Clear();
        }
    }

    private bool CanAddAllAction() {
        return !Users.IsNullOrEmpty();
    }
}