using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class OperationViewModel : DialogViewModelBase<Operation, PridContext>
    {
        public OperationViewModel(Operation operation) {
            // initialisation des propriétés
            Tricount = Context.Tricounts.Find(operation.TricountId);
            Operation = operation;
            WindowTitle = operation.Title == null ? "Add Operation" : "Edit Operation";
            Amount = Operation.Title != null ? $"{Operation.Amount:F2}" : "0,00";
            Title = Operation.Title ?? "";
            OperationDate = Operation.Title == null ? DateTime.Today : Operation.OperationDate;
            Button = Operation.Title == null ? "Add" : "Save";
            Visible = Operation.Title == null ? Visibility.Hidden : Visibility.Visible;
            SelectedTemplate = new Template() { Title = "-- Choose a template --" };
            Participants = GetUsersTricount();
            Templates = GetTemplatesTricount();
            Templates.Add(SelectedTemplate);
            CurrentUser = App.CurrentUser;
            Initiator = operation.Title == null ? CurrentUser : Context.Users.Find(Operation.InitiatorId);
            NoTemplates = Templates.Any();
            Repartitions = new ObservableCollectionFast<RepartitionsViewModel>();
            FillRepartitionsViewModels();

            // initialisation des commandes 
            SaveCommand = new RelayCommand(SaveAction, () => !HasErrors && Error == "");
            ApplyTemplate = new RelayCommand(ApplyAction,
                // vérification pour éviter Null Exception
                () => SelectedTemplate == null
                    ? SelectedTemplate != null
                    : SelectedTemplate.Title != "-- Choose a template --");
            SaveTemplate = new RelayCommand(SaveTemplateAction, () => !HasErrors && Error == "");
            DeleteCommand = new RelayCommand(DeleteAction);
        }

        // Commandes
        public ICommand SaveCommand { get; set; }
        public ICommand ApplyTemplate { get; set; }
        public ICommand SaveTemplate { get; set; }
        public ICommand DeleteCommand { get; set; }

        // Attributes
        private ObservableCollectionFast<RepartitionsViewModel> _repartitions;
        private readonly User _currentUser;
        private User _initiator;
        private Tricount _tricount;
        private readonly Operation _operation;
        private Template _selectedTemplate;
        private string _amount;
        private bool _noTemplates;
        private DateTime _operationDate;
        private string _error;
        private string _windowTitle;
        private readonly string _button;
        private Visibility _visible;
        private List<User> _participants;
        private List<Template> _templates;
        private string _title;

        // Properties
        public ObservableCollectionFast<RepartitionsViewModel> Repartitions {
            get => _repartitions;
            set => SetProperty(ref _repartitions, value);
        }

        public List<User> Participants {
            get => _participants;
            set => SetProperty(ref _participants, value);
        }


        public List<Template> Templates {
            get => _templates;
            set => SetProperty(ref _templates, value);
        }

        public string Error {
            get => _error;
            private set => SetProperty(ref _error, value);
        }

        public string WindowTitle {
            get => _windowTitle;
            private set => SetProperty(ref _windowTitle, value);
        }

        public bool NoTemplates {
            get => _noTemplates;
            set => SetProperty(ref _noTemplates, value);
        }

        public Operation Operation {
            get => _operation;
            init => SetProperty(ref _operation, value);
        }

        public Template SelectedTemplate {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value);
        }

        public DateTime OperationDate {
            get => _operationDate;
            set => SetProperty(ref _operationDate, value, () => Validate());
        }

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        private new User CurrentUser {
            get => _currentUser;
            init => SetProperty(ref _currentUser, value);
        }

        public User Initiator {
            get => _initiator;
            set => SetProperty(ref _initiator, value);
        }

        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => {
                Validate();
                ApplicationRoot.NotifyColleagues(App.Messages.AMOUNT_CHANGED);
            });
        }

        public string Title {
            get => _title;
            set => SetProperty(ref _title, value, () => {
                Validate();
            });
        }

        public string Button {
            get => _button;
            private init => SetProperty(ref _button, value);
        }

        public Visibility Visible {
            get => _visible;
            set => SetProperty(ref _visible, value);
        }


        // Méthodes Commandes

        // Save/Add
        public override void SaveAction() {
            if (!Validate()) {
                return;
            }

            Operation.Title = Title;
            Operation.Amount = double.Parse(Amount);
            Operation.OperationDate = OperationDate;
            Operation.InitiatorId = Initiator.UserId;
            if (!Tricount.GetOperations().ToList().Contains(Operation)) {
                Context.Add(Operation);
            }

            SaveWeights();
            Context.SaveChanges();
            RaisePropertyChanged();
            NotifyColleagues(App.Messages.MSG_OPERATION_CHANGED, Operation);
            NotifyColleagues(App.Messages.MSG_OPERATION_TRICOUNT_CHANGED, Tricount);
            Close();
        }

        // Save les poids de l'opération
        private void SaveWeights() {
            if (Repartitions == null) {
                return;
            }

            foreach (var repartition in Repartitions) {
                User user = repartition.User;
                // Si paire operation-user existante parmi les répartitions -> modifications des poids
                if (Operation.Repartitions.Any(r => r.UserId == user.UserId && r.OperationId == Operation.Id)) {
                    UpdateWeight(user.UserId, repartition.Weight);
                } else if (repartition.Weight > 0) {
                    // Nouvelle répartition si paire operation-user inexistante parmi les répartitions
                    Operation.Repartitions.Add(new Repartition(Operation.Id, user.UserId, repartition.Weight));
                }
            }
        }

        // Modifie poid associé à un user dans une répétition
        private void UpdateWeight(int userId, int newWeight) {
            // Rechercher la répartition correspondante pour l'utilisateur spécifié
            Repartition repartition =
                Operation.Repartitions.FirstOrDefault(r => r.UserId == userId && r.OperationId == Operation.Id);
            if (repartition == null) {
                return;
            }

            // Mettre à jour le poids de l'utilisateur
            repartition.Weight = newWeight;
            Context.SaveChanges();
        }


        // Delete 
        private void DeleteAction() {
            Operation.Delete();
            NotifyColleagues(App.Messages.MSG_OPERATION_CHANGED, Operation);
            NotifyColleagues(App.Messages.MSG_OPERATION_TRICOUNT_CHANGED, Tricount);
            NotifyColleagues(App.Messages.MSG_DELETED);
            Close();
        }

        private void SaveTemplateAction() {
        }

        // Applique la template selectionnée 
        private void ApplyAction() {
            // vérifications 
            if (SelectedTemplate != null && Repartitions != null) {
                string title = SelectedTemplate.Title;
                if (Tricount.GetTemplateByTitle(title) != null) {
                    // récupération template sélectionné
                    var t = Tricount.GetTemplateByTitle(title);
                    // association user-weight dans un dictionnaire
                    Dictionary<string, int> UserWeight = t.GetUsersAndWeightsByTricountId();
                    // attribution des valeurs sinon 0 si user inexistant dans la template
                    foreach (var repartition in Repartitions) {
                        if (UserWeight.ContainsKey(repartition.User.FullName))
                            repartition.Weight = UserWeight[repartition.User.FullName];
                        else
                            repartition.Weight = 0;
                    }
                }
            }
        }

        // Méthodes de validation
        public override bool Validate() {
            ClearErrors();
            ValidateTitle();
            IsValidAmount();
            IsValidDate();
            AddErrors(Operation.Errors);
            Error = !ValidateCheckBoxes() ? "You must check at least one participant !" : "";

            return !HasErrors;
        }

        private void ValidateTitle() {
            ClearErrors(); // Efface les erreurs de validation précédentes

            if (string.IsNullOrWhiteSpace(Title)) // Vérifie si le titre est vide ou null
                AddError(nameof(Title), "required"); // Ajoute une erreur si le titre est requis
            else if (Title.Length < 3) // Vérifie si le titre a une longueur inférieure à 3 caractères
                AddError(nameof(Title), "length must be >= 3"); // Ajoute une erreur si le titre est trop court

        }

        // Check if at least one checkbox is checked
        private bool ValidateCheckBoxes() {
            return Repartitions == null || Repartitions.Any(r => r.IsChecked);
        }

        // Return Users of the Operation's Tricount.
        protected internal List<User> GetUsersTricount() {
            Tricount tricount = Tricount;
            List<User> participants = new List<User>();
            foreach (User user in tricount.GetParticipants()) {
                participants.Add(user);
            }

            return participants;
        }

        // Return Templates of the Operation's Tricount.
        protected internal List<Template> GetTemplatesTricount() {
            Tricount tricount = Tricount;
            List<Template> templates = new List<Template>();
            foreach (Template template in tricount.GetTemplates()) {
                templates.Add(template);
            }

            return templates;
        }

        // Validation méthode for string amount -> we need a string because of textbox
        private void IsValidAmount() {
            if (Amount is { Length: > 0 }) {
                if (!double.TryParse(Amount, out double value))
                    AddError(nameof(Amount), "Not an Integer");
                if (double.Parse(Amount) < 0.01) {
                    AddError(nameof(Amount), "minimum 1 cent");
                }
            } else
                AddError(nameof(Amount), "Can't be empty !");
        }

        private void IsValidDate() {
            if (OperationDate < Tricount.CreatedAt) {
                AddError(nameof(OperationDate), $"Cannot be before {Tricount.CreatedAt:dd-MM-yyyy}.");
            }

            if (OperationDate > DateTime.Today) {
                AddError(nameof(OperationDate), "cannot be in the future.");
            }
        }

        public void FillRepartitionsViewModels() {
            foreach (var user in Participants) {
                if (Operation.Repartitions.Any(r => r.UserId == user.UserId && r.OperationId == Operation.Id)) {
                    Repartition repartition = Operation.Repartitions.FirstOrDefault(r =>
                        r.UserId == user.UserId && r.OperationId == Operation.Id);
                    Repartitions.Add(new RepartitionsViewModel(repartition, this));
                } else {
                    Repartitions.Add(new RepartitionsViewModel(new Repartition(Operation.Id, user.UserId, 0), this));
                }
            }

            // mise à jour montants de chaque répartition une fois la liste de répartitions complète
            NotifyColleagues(App.Messages.AMOUNT_CHANGED);
            // Validation une fois liste de répartitions complète pour bien vérifier chaque checkbox
            Validate();
        }

        protected internal void Close() {
            NotifyColleagues(App.Messages.MSG_CLOSE_OPERATION_WINDOW, Operation);
        }
    }
}