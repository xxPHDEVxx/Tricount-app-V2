using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class Session1OperationViewmodel : ViewModelCommon
    {
        public Session1OperationViewmodel(Operation operation) {
            Operation = operation;
        }

        private Boolean _ischecked;

        public Boolean Ischecked {
            get => _ischecked;
            set => SetProperty(ref _ischecked, value);
        }

        private Operation _operation;

        public Operation Operation {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }
    }

    public class Session1ViewModel : ViewModelCommon
    {
        public Session1ViewModel() {
            GetUsers();

            // initialisation des commandes 
            AddCommand = new RelayCommand(AddAction);
            
            // refresh
            
            Register(App.Messages.SESSION1_DATA_CHANGED, OnRefreshData);
        }

        // Commands
        public ICommand AddCommand { get; set; }

        // Attributes and properties
        private ObservableCollectionFast<Session1OperationViewmodel> _operations =
            new ObservableCollectionFast<Session1OperationViewmodel>();

        public ObservableCollectionFast<Session1OperationViewmodel> Operations {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        private List<User> _users;

        public List<User> Users {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private List<Tricount> _tricounts;

        public List<Tricount> Tricounts {
            get => _tricounts;
            set => SetProperty(ref _tricounts, value);
        }

        private User _selectedUser;

        public User SelectedUser {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value, () => {
                GetTricounts();
                IsSelected = SelectedUser != null;
                AddEnable = SelectedUser != null && SelectedTricount != null;
            });
        }

        private Tricount _selectedTricount;

        public Tricount SelectedTricount {
            get => _selectedTricount;
            set => SetProperty(ref _selectedTricount, value, () => {
                GetOperations();
                Console.WriteLine(Operations.Count);
                AddEnable = SelectedUser != null && SelectedTricount != null;
            });
        }

        private Boolean _isSelected;

        public Boolean IsSelected {
            get => SelectedUser != null;
            set => SetProperty(ref _isSelected, value);
        }

        private Boolean _addEnable;

        public Boolean AddEnable {
            get => SelectedUser != null && SelectedTricount != null;
            set => SetProperty(ref _addEnable, value);
        }

        // Méthods

        private void AddAction() {
            if (Operations != null) {
                // add participation 
                SelectedUser.Subscriptions.Add(new Subscription(SelectedUser.UserId, SelectedTricount.Id));
                // add repartition for each operation checked
                foreach (var operation in Operations) {
                    if (operation.Ischecked)
                        operation.Operation.Repartitions.Add(new Repartition(operation.Operation.Id, SelectedUser.UserId, 1));
                }
                Context.SaveChanges();
                NotifyColleagues(App.Messages.SESSION1_DATA_CHANGED);
            }
        }

        private void GetUsers() {
            var users = Context.Users
                // attention pas utiliser collection sans count sinon( linq gère pas collection)
                .OrderByDescending(u => u.Subscriptions.Count)
                .ThenBy(u => u.FullName)
                .Select(u => u);
            Users = users.ToList();
        }

        // récupère tricounts auquels l'utilisateur selectionné n'a pas accès
        private void GetTricounts() {
            if (SelectedUser != null) {
                Tricounts = SelectedUser.GetTricountsWithNoParticipation().ToList();
            }
        }

        private void GetOperations() {
            Operations.Clear();
            if (SelectedTricount != null) {
                var operations = SelectedTricount.GetOperations();

                foreach (var operation in operations) {
                    Operations.Add(new Session1OperationViewmodel(operation));
                }
            }
        }
        
        protected override void OnRefreshData() {
            Tricounts = SelectedUser.GetTricounts().ToList();
            GetUsers();
        }
    }
}