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
    private ObservableCollectionFast<User> _users;
    public ObservableCollectionFast<User> Users { 
        get => _users;
        set => SetProperty(ref _users, value);
    }

    private User _selectedUser;

    public User SelectedUser {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    private ObservableCollectionFast<CardParticipantViewModel> _participants;

    public ObservableCollectionFast<CardParticipantViewModel> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }
    
    private ObservableCollectionFast<CardTemplateViewModel> _templates;

    public ObservableCollectionFast<CardTemplateViewModel> Templates {
        get => _templates;
        set => SetProperty(ref _templates, value);
    }
    public string CreatedBy {

        get => IsNew ? $"Created By {User.FullName} on {DateTime.Now.ToString("dd/MM/yyyy")}": $"Created By {Tricount.Creator.FullName} on {Tricount.CreatedAt.ToString("dd/MM/yyyy")}";
    }
    public DateTime DateLast => Tricount.GetLastDate();

    public string TitleHeader { get => Tricount?.Title ?? "<New Tricount>"; }
    public string DescriptionHeader { get => Tricount?.Description ?? "No description"; }
    public string Title {
        get => Tricount?.Title;
        set => SetProperty(Tricount.Title, value, Tricount, (t, v) => {
            t.Title = v;
            Validate();
            NotifyColleagues(App.Messages.MSG_TITLE_CHANGED, Tricount);
        });
    }


    public string Description {
        get => Tricount?.Description ;
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
        
        AddTemplate = new RelayCommand(AddTemplateAction, CanAddTemplateAction);
        
        OnRefreshData();
        RaisePropertyChanged();

        Console.WriteLine(Participants.Any(c => c.User == GetCurrentUser()));



    }



    protected override void OnRefreshData() {
        if (IsNew) {
            IQueryable<User> users = Context.Users
                .Where(u => u.UserId != CurrentUser.UserId && u.Role == Role.User)
                .OrderBy(m => m.FullName);
            Users = new ObservableCollectionFast<User>(users.Select(u => u));

           
            Participants = new ObservableCollectionFast<CardParticipantViewModel>();

            Participants.Add(new CardParticipantViewModel(this, CurrentUser));
            Date = DateTime.Now;
        } else {
            Tricount = Model.Tricount.GetTricountByTitle(Tricount.Title);
            Date = Tricount.CreatedAt;
            //on récupère les participants du tricont
            IQueryable<User> allPart = Tricount.GetParticipants().ToList().AsQueryable();
            Participants = new ObservableCollectionFast<CardParticipantViewModel>(
               allPart
                .Select(u => new CardParticipantViewModel(this, u))
                );

            //on récupere les users qui ne participent pas
            IQueryable<User> part = Tricount.GetParticipants();

            IQueryable<User> users = Context.Users
                .Where(u => u.UserId != CurrentUser.UserId && u.Role == Role.User)
                .OrderBy(m => m.FullName);
            
            Users = new ObservableCollectionFast<User>(users.Except(part));

            // récupere les templates
            IQueryable<Template> templates = Tricount.GetTemplates().ToList().AsQueryable();

            Templates = new ObservableCollectionFast<CardTemplateViewModel>(templates
                .Select(t => new CardTemplateViewModel(this, t))
                );
        }
    }
    public override void SaveAction() {
        // Add propriétés au tricount 
        if (IsNew) {
            Tricount = new Tricount(Title, Description, Date, User);
            Context.Tricounts.Add(Tricount);
        }

        AddSubscriptions();
        Context.SaveChanges();
        RaisePropertyChanged();
        NotifyColleagues(App.Messages.MSG_TRICOUNT_CHANGED, Tricount);
    }

    // Add Subscriptions
    public void AddSubscriptions() {
        if (Tricount != null) {
            if (IsNew) {
                foreach (var user in Participants) {
                    Tricount.Subscriptions.Add(new Subscription {
                        UserId = user.User.UserId,
                        TricountId = Tricount.Id
                    });
                }

            } else {

                var currentSubscriptions = Tricount.Subscriptions.ToList();

                //Si l'user n'est pas dans les subscrptions on l'ajoute
                foreach (var user in Participants) {
                    if (!currentSubscriptions.Any(s => s.UserId == user.User.UserId)) {
                        Tricount.Subscriptions.Add(new Subscription {
                            UserId = user.User.UserId,
                            TricountId = Tricount.Id
                        });
                    }
                }

                // depuis les participants, si on ne retrouve pas l'user on le supprime
                foreach (var subscription in currentSubscriptions) {
                    if (!Participants.Any(p => p.User.UserId == subscription.UserId)) {
                        Tricount.Subscriptions.Remove(subscription);
                    }
                }

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

        Participants.Add(new CardParticipantViewModel( this, SelectedUser));
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
             if (!Participants.Any(c => c.User == currentUser)) {
                Participants.Add(new CardParticipantViewModel(this, currentUser));
            }
        }
    }

    private bool CanAddMySelfAction() {
        var currentUser = GetCurrentUser();
        return !Participants.Any(c => c.User == GetCurrentUser());
    }

    private void AddAllAction() {
        foreach (var user in Users.ToList()) {
            if (!Participants.Any(c => c.User == user)) {
                Participants.Add(new CardParticipantViewModel(this, user));
            }
            Users.Clear();
        }
    }
    

    private bool CanAddAllAction() {
        return !Users.IsNullOrEmpty();
    }

    //add template et canAdd
    private void AddTemplateAction() {
        //scoop de 3 - pas implémenté
    }


    private bool CanAddTemplateAction() {
        return !IsNew;
    }
}