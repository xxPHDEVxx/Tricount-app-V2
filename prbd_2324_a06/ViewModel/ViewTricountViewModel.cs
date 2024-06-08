using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class ViewTricountViewModel : ViewModelCommon
    {
        public ViewTricountViewModel(Tricount tricount) : base() {
            // Initialisation
            Tricount = tricount;
            Visible = App.CurrentUser.Role is Role.Administrator || App.CurrentUser.UserId == Tricount.CreatorId
                ? Visibility.Visible
                : Visibility.Hidden;
            OnRefreshData();
            DisplayOperation = new RelayCommand<TricountCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_DISPLAY_OPERATIONS, vm.Tricount);
            });
            EditTricount = new RelayCommand(() => {
                NotifyColleagues(App.Messages.MSG_OPEN_TRICOUNT, Tricount);
            });
            DeleteTricount = new RelayCommand(DeleteAction);
            OpenEditOperation = new RelayCommand<OperationCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_OPEN_OPERATION, vm.Operation);
            });
            OpenNewOperation = new RelayCommand<OperationCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_OPEN_NEW_OPERATION, new Operation(tricount.Id));
            });
            Register<Operation>(App.Messages.MSG_OPERATION_CHANGED, operation => OnRefreshData());
            // Reset
            Register(App.Messages.MSG_RESET, () => OnRefreshData());
        }

        private ObservableCollection<OperationCardViewModel> _operations;
        private ObservableCollection<UserBalanceViewModel> _users;
        private Tricount _tricount;
        private string _filter;
        private Visibility _visible;

        // Properties

        public ObservableCollection<OperationCardViewModel> Operations {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        public ObservableCollection<UserBalanceViewModel> Users {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public string Title {
            get => Tricount.Title;
            set => SetProperty(Tricount.Title, value, Tricount, (tr, t) => {
            });
        }

        public string Description {
            get => Tricount.Description;
            set => SetProperty(Tricount.Description, value, Tricount, (tr, d) => {
            });
        }

        public string Creator {
            get => User.GetUserNameById(Tricount.CreatorId);
            set => SetProperty(User.GetUserNameById(Tricount.CreatorId), value, Tricount, (t, c) => {
                OnRefreshData();
            });
        }

        public DateTime CreatedAt {
            get => Tricount.CreatedAt;
            init => SetProperty(Tricount.CreatedAt, value, Tricount
                , (t, d) => { });
        }

        public Visibility Visible {
            get => _visible;
            set => SetProperty(ref _visible, value);
        }

        // Commandes
        public ICommand ClearFilter { get; set; }
        public ICommand DisplayOperation { get; set; }
        public ICommand OpenNewOperation { get; set; }
        public ICommand OpenEditOperation { get; set; }
        public ICommand EditTricount { get; set; }
        public ICommand DeleteTricount { get; set; }


        // Méthodes 

        // Delete

        private void DeleteAction() {
            Tricount.Delete();
            NotifyColleagues(App.Messages.MSG_TRICOUNT_CHANGED, Tricount);
            NotifyColleagues(App.Messages.MSG_DELETED);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
        }

        // Permet le Refresh
        protected override void OnRefreshData() {
            if (Tricount == null) return;

            // Récupérer les participants par ordre alphabétique
            IQueryable<User> participants = Tricount.GetParticipants().OrderBy(u => u.FullName);

            // Mettre à jour les données avec les utilisateurs participants
            Users = new ObservableCollection<UserBalanceViewModel>(participants.Select(u =>
                new UserBalanceViewModel(u, Tricount)));

            IQueryable<Operation> operations = Tricount.GetOperations()
                .OrderByDescending(o => o.OperationDate)
                .ThenByDescending(o => o.Id); // Tri secondaire par OperationId

            Operations = new ObservableCollection<OperationCardViewModel>(operations.Select(o =>
                new OperationCardViewModel(o)));
        }
    }
}