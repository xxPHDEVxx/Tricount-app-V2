using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Controls;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class EditOperationViewModel : ViewModelCommon
    {
        // ajouter en parametre Tricount pour lier au reste du code
        public EditOperationViewModel() : base() {
            // initialisation des propriétés
            //Tricount = tricount;
            Tricount = Context.Tricounts.Find(4); // pour les tests
            Operation = Context.Operations.Find(2); // pour les tests
            Amount = Operation.Amount.ToString();
            OperationDate = Operation.OperationDate;
            NoTemplates = GetTemplatesTricount().Any();
            // Une fois liée au reste du code à décommenté
            // CurrentUser = App.CurrentUser.FullName;
            CurrentUser = Context.Users.Find(2);
            CheckBoxItems = new ObservableCollectionFast<CheckBox>();

            // initialisation des commandes 
            SaveCommand = new RelayCommand(SaveAction, () => !HasErrors && Error == "");
            ApplyTemplate = new RelayCommand(ApplyAction);
            SaveTemplate = new RelayCommand(SaveTemplateAction, () => !HasErrors);
            DeleteCommand = new RelayCommand(DeleteAction);
        }

        // Commandes
        public ICommand SaveCommand { get; set; }
        public ICommand ApplyTemplate { get; set; }
        public ICommand SaveTemplate { get; set; }
        public ICommand DeleteCommand { get; set; }

        // Attributes
        private ObservableCollectionFast<CheckBox> _checkBoxItems;
        private User _currentUser;
        private Tricount _tricount;
        private Operation _operation;
        private object _selectedTemplate;
        private string _amount;
        private bool _noTemplates;
        private DateTime _operationDate;
        private string _error;

        // Properties
        public ObservableCollectionFast<CheckBox> CheckBoxItems {
            get => _checkBoxItems;
            set => SetProperty(ref _checkBoxItems, value);
        }

        public string Error {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public bool NoTemplates {
            get => _noTemplates;
            set => SetProperty(ref _noTemplates, value);
        }

        public Operation Operation {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }

        public object SelectedTemplate {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value);
        }

        public DateTime StartDate {
            get => Tricount.CreatedAt;
            set {
                DateTime tricountCreatedAt = Tricount.CreatedAt;
                SetProperty(ref tricountCreatedAt, value);
            }
        }

        public DateTime OperationDate {
            get => _operationDate;
            set => SetProperty(ref _operationDate, value);
        }

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public new User CurrentUser {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => Validate());
        }

        public string Title {
            get => Operation?.Title;
            set => SetProperty(Operation.Title, value, Operation, (o, t) => {
                o.Title = t;
                Validate();
            });
        }

        // Méthodes Commandes

        // Edit
        public override void SaveAction() {
            if (Validate()) {
                Operation.Title = Title;
                Operation.Amount = double.Parse(Amount);
                Operation.OperationDate = OperationDate;
                // ajouter weight, initiator, templates quand binding seront ok
                Context.SaveChanges();
                RaisePropertyChanged();
                NotifyColleagues(App.Messages.MSG_EDIT_OPERATION);
            }
        }

        // Delete 
        private void DeleteAction() {
            Operation.Delete();
            NotifyColleagues(App.Messages.MSG_DELETE_OPERATION);
            NotifyColleagues(App.Messages.MSG_CLOSE_WINDOW);
        }

        private void SaveTemplateAction() {
            if (Validate()) {
            }
        }

        private void ApplyAction() {
            if (SelectedTemplate != null) {
            }
        }

        // Méthodes de validation
        public override bool Validate() {
            ClearErrors();
            Operation.Validate();
            IsValidAmount();
            AddErrors(Operation.Errors);
            if (!ValidateCheckBoxes()) {
                Error = "You must check at least one participant !";
            } else {
                Error = "";
            }

            return !HasErrors;
        }

        // Check if at least one checkbox is checked
        public bool ValidateCheckBoxes() {
            if (CheckBoxItems != null) {
                return CheckBoxItems.Any(item => item.IsChecked != null && (bool)item.IsChecked);
            }

            return true;
        }

        public void isSelectedTemplate() {
            if (true) {
            }
        }

        // Return Users of the Operation's Tricount.
        protected internal List<User> GetUsersTricount() {
            Tricount tricount = Tricount;
            List<User> participants = new List<User>();
            foreach (var user in tricount.GetParticipants()) {
                participants.Add(user);
            }

            return participants;
        }

        // Return Templates of the Operation's Tricount.
        protected internal List<Template> GetTemplatesTricount() {
            Tricount tricount = Tricount;
            List<Template> templates = new List<Template>();
            foreach (var template in tricount.GetTemplates()) {
                templates.Add(template);
            }

            return templates;
        }

        // Validation méthode for string amount -> we need a string because of textbox
        public void IsValidAmount() {
            ClearErrors();
            if (!double.TryParse(Amount, out double value))
                AddError(nameof(Amount), "Not an Integer");
            if (double.Parse(Amount) < 0.01) {
                AddError(nameof(Amount), "minimum 1 cent");
            }
        }
    }
}