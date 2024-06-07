using NumericUpDownLib;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Controls;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class AddOperationViewModel : ViewModelCommon
    {
        // ajouter en parametre Tricount pour lier au reste du code
        public AddOperationViewModel(Operation operation) : base() {
            //Tricount = tricount;
            Tricount = Context.Tricounts.Find(operation.TricountId);
            CurrentUser = App.CurrentUser;
            CheckBoxItems = new ObservableCollectionFast<CheckBox>();
            Numerics = new ObservableCollectionFast<NumericUpDown>();
            TextBlocks = new ObservableCollectionFast<TextBlock>();
            Initiator = new ComboBoxItem {
                Content = CurrentUser.FullName
            };
            NoTemplates = GetTemplatesTricount().Any();
            // initialisation des commandes 
            AddCommand = new RelayCommand(AddAction,
                () => !HasErrors && Error == "");
            ApplyTemplate = new RelayCommand(ApplyAction,
                // vérification pour éviter Null Exception
                () => SelectedTemplate == null
                    ? SelectedTemplate != null
                    : SelectedTemplate.Content.ToString() != "-- Choose a template --");
            SaveTemplate = new RelayCommand(SaveTemplateAction, () => !HasErrors && Error == "");
            Operation = new Operation();
        }

        // Commandes
        public ICommand AddCommand { get; set; }
        public ICommand ApplyTemplate { get; set; }
        public ICommand SaveTemplate { get; set; }

        // Attributes
        private ObservableCollectionFast<CheckBox> _checkBoxItems;
        private ObservableCollectionFast<NumericUpDown> _numerics;
        private ObservableCollectionFast<TextBlock> _textBlocks;
        private ComboBoxItem _initiator;
        private ComboBoxItem _selectedTemplate;
        private Operation _operation;
        private string _amount;
        private User _currentUser;
        private Tricount _tricount;
        private string _error;
        private bool _noTemplates;

        // Properties
        public ObservableCollectionFast<CheckBox> CheckBoxItems {
            get => _checkBoxItems;
            private init => SetProperty(ref _checkBoxItems, value);
        }

        public ObservableCollectionFast<NumericUpDown> Numerics {
            get => _numerics;
            private init => SetProperty(ref _numerics, value);
        }

        public ObservableCollectionFast<TextBlock> TextBlocks {
            get => _textBlocks;
            private init => SetProperty(ref _textBlocks, value);
        }

        public bool NoTemplates {
            get => _noTemplates;
            set => SetProperty(ref _noTemplates, value);
        }

        public ComboBoxItem SelectedTemplate {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value);
        }

        public ComboBoxItem Initiator {
            get => _initiator;
            set => SetProperty(ref _initiator, value);
        }

        public string Error {
            get => _error;
            private set => SetProperty(ref _error, value);
        }

        public DateTime StartDate {
            get => Tricount.CreatedAt;
            set {
                DateTime tricountCreatedAt = Tricount.CreatedAt;
                SetProperty(ref tricountCreatedAt, value);
            }
        }

        private Tricount Tricount {
            get => _tricount;
            init => SetProperty(ref _tricount, value);
        }

        public new User CurrentUser {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => {
                Validate();
                CalculAmount();
            });
        }

        public string Title {
            get => Operation?.Title;
            set => SetProperty(Operation.Title, value, Operation, (o, t) => {
                o.Title = t;
                Validate();
            });
        }

        private Operation Operation {
            get => _operation;
            init => SetProperty(ref _operation, value);
        }

        // Méthodes Commandes

        // Add
        private void AddAction() {
            if (Validate()) {
                Operation.Title = Title;
                Operation.TricountId = Tricount.Id;
                Operation.Amount = double.Parse(Amount);
                Operation.OperationDate = DateTime.Today;
                Operation.InitiatorId = User.GetUserByName(Initiator.Content.ToString()).UserId;
                Context.Add(Operation);
                SaveWeights();
                Context.SaveChanges();
                RaisePropertyChanged();
                NotifyColleagues(App.Messages.MSG_OPERATION_CHANGED, Operation);
                Close();
            }
        }

        // Save les poids de l'opération
        private void SaveWeights() {
            if (Numerics != null) {
                foreach (var item in Numerics) {
                    User user = User.GetUserByName(item.Name);
                    // Si paire operation-user existante parmi les répartitions -> modifications des poids
                    if (Operation.Repartitions.Any(r => r.UserId == user.UserId && r.OperationId == Operation.Id))
                        UpdateWeight(user.UserId, item.Value);
                    else {
                        // Nouvelle répartition si paire operation-user inexistante parmi les répartitions
                        Operation.Repartitions.Add(new Repartition(Operation.Id, user.UserId, item.Value));
                    }
                }
            }
        }

        // Modifie poid associé à un user dans une répétition
        private void UpdateWeight(int userId, int newWeight) {
            // Rechercher la répartition correspondante pour l'utilisateur spécifié
            Repartition repartition =
                Operation.Repartitions.FirstOrDefault(r => r.UserId == userId && r.OperationId == Operation.Id);

            if (repartition != null) {
                // Mettre à jour le poids de l'utilisateur
                repartition.Weight = newWeight;
                Context.SaveChanges();
            }
        }

        private void SaveTemplateAction() {
        }

        // Applique la template selectionnée 
        private void ApplyAction() {
            // vérifications 
            if (SelectedTemplate != null && Numerics != null) {
                string title = SelectedTemplate.Content.ToString();
                if (Tricount.GetTemplateByTitle(title) != null) {
                    // récupération template sélectionné
                    var t = Tricount.GetTemplateByTitle(title);
                    // association user-weight dans un dictionnaire
                    Dictionary<string, int> UserWeight = t.GetUsersAndWeightsByTricountId();
                    // attribution des valeurs sinon 0 si user inexistant dans la template
                    foreach (var item in Numerics) {
                        if (UserWeight.ContainsKey(item.Name))
                            item.Value = UserWeight[item.Name];
                        else
                            item.Value = 0;
                    }
                }
            }
        }

        // Méthode de validation
        public override bool Validate() {
            ClearErrors();
            Operation.Validate();
            IsValidAmount();
            AddErrors(Operation.Errors);
            Error = !ValidateCheckBoxes() ? "You must check at least one participant !" : "";

            return !HasErrors;
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

        // Check if at least one checkbox is checked
        private bool ValidateCheckBoxes() {
            return CheckBoxItems == null
                   || CheckBoxItems.Any(item => item.IsChecked != null && (bool)item.IsChecked);
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

        public void CalculAmount() {
            if (Amount is { Length: > 0 }) {
                int totalWeight = 0;
                if (Numerics != null && TextBlocks != null) {
                    int[] weights = new int[Numerics.Count];

                    // Calcul du total des poids
                    var i = 0;
                    foreach (var item in Numerics) {
                        totalWeight += item.Value;
                        weights[i] = item.Value;
                        i++;
                    }

                    // insertion montants dans textblock
                    i = 0;
                    int part = totalWeight < 1 ? int.Parse(Amount) * totalWeight : int.Parse(Amount) / totalWeight;
                    foreach (var item in TextBlocks) {
                        item.Text = $"{part * weights[i]:F2} €";
                        i++;
                    }
                }
            } else {
                AddError(nameof(Amount), "Can't be empty !");
            }
        }

        protected internal void Close() {
            NotifyColleagues(App.Messages.MSG_CLOSE_OPERATION_WINDOW, Operation);
        }
    }
}