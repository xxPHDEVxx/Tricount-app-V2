using Microsoft.IdentityModel.Tokens;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Linq;
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


    // Attributs et propriétés
    public ObservableCollectionFast<User> Users { get; } = new ObservableCollectionFast<User>();

    private User _selectedUser;

    public User SelectedUser {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    private ObservableCollection<CardParticipantViewModel> _participants;

    public ObservableCollection<CardParticipantViewModel> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }
    public string CreatedBy {

        get => IsNew ? $"Created By {User.FullName} on {DateTime.Now.ToString("dd/MM/yyyy")}": $"Created By {Tricount.Creator.FullName} on {Tricount.CreatedAt.ToString("dd/MM/yyyy")}";
    } 
    public string TricountTitle {
        get => IsNew ? "New Tricount - no description" : $"{Tricount.Title} {Tricount.Description?? "No Description"}" ;
    }
    public string Title {
        get => Tricount?.Title;
        set => SetProperty(Tricount.Title, value, Tricount, (t, v) => {
            t.Title = v;
            Validate();
            NotifyColleagues(App.Messages.MSG_TITLE_CHANGED, Tricount);
        });
    }


    public string Description {
        get => Tricount?.Description;
        set => SetProperty(Tricount.Description, value, Tricount, (t, v) => {
            t.Description = v;
            Validate();
        });
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

    private bool _isNew;

    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }


    public TricountDetailViewModel(Tricount tricount, bool isNew) : base() {
        Tricount = tricount;
        IsNew = isNew;



        User = CurrentUser;

        SaveCommand = new RelayCommand(SaveAction, CanSaveAction);
        CancelCommand = new RelayCommand(CancelAction, CanCancelAction);
        AddCommand = new RelayCommand(AddParticipantAction, CanAddParticipantAction);
        AddMySelf = new RelayCommand(AddMySelfAction,CanAddMySelfAction);
        AddEvery = new RelayCommand(AddAllAction, CanAddAllAction);

        OnRefreshData();
        RaisePropertyChanged();


    }



    protected override void OnRefreshData() {
        if (IsNew) {
            Users.RefreshFromModel(Context.Users
                .Where(u => u.UserId != CurrentUser.UserId && u.Role == Role.User)
                .OrderBy(m => m.FullName)
            );
            Participants = new ObservableCollection<CardParticipantViewModel>();

            Participants.Add(new CardParticipantViewModel(this, CurrentUser));
        } else {
            Tricount = Model.Tricount.GetTricountByTitle(Tricount.Title);

            IQueryable<User> allPart = Tricount.GetParticipants().ToList().AsQueryable();
            Participants = new ObservableCollection<CardParticipantViewModel>(
               allPart
                .Select(u => new CardParticipantViewModel(this, u))
                );
            //Participants = new ObservableCollection<CardParticipantViewModel>();
            Users.RefreshFromModel(Context.Users
            .Where(u => u.UserId != CurrentUser.UserId && u.Role == Role.User).OrderBy(m => m.FullName));
        }
    }
    public override void SaveAction() {
        // Add propriétés au tricount 
        if (IsNew) {
            Tricount = new Tricount(Title, Description, DateTime.Today, User);
            Context.Tricounts.Add(Tricount);
            AddSubscriptions();
            IsNew = false;
        }

        // ajouter code pour edit
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_TRICOUNT_CHANGED, Tricount);
    }

    // Add Subscriptions
    public void AddSubscriptions() {
        if (Tricount != null) {
            foreach (var user in Participants) {
                Tricount.Subscriptions.Add(new Subscription {
                    UserId = user.User.UserId,
                    TricountId = Tricount.Id
                });
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
            Participants.Add(new CardParticipantViewModel(this, SelectedUser));
            Users.Remove(SelectedUser);
            NotifyColleagues(App.Messages.MSG_PARTICIPANT_ADDED, SelectedUser);
            Console.WriteLine(Participants.Count);
        }

        if (Participants == null || SelectedUser == null || Participants.Contains(new CardParticipantViewModel(this, SelectedUser))) {
            return;
        }

        Participants.Add(new CardParticipantViewModel(  this, SelectedUser));
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
            // if (!Participants.Contains(currentUser)) {
            Participants.Add(new CardParticipantViewModel(this, currentUser));
            //}
        }
    }

    private bool CanAddMySelfAction() {
        var currentUser = GetCurrentUser();
        return IsNew;
    }

    private void AddAllAction() {
        if (IsNew) {
            foreach (var user in Users) {
               // if (!Participants.Contains(user)) {
                    Participants.Add(new CardParticipantViewModel(this, user));
                //}
            }
            Users.Clear();
        }
    }

    private bool CanAddAllAction() {
        return !Users.IsNullOrEmpty();
    }
}