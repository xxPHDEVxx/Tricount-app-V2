using Microsoft.Extensions.Options;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class AddOperationViewModel : ViewModelCommon
    {
        // ajouter en parametre Tricount pour lier au reste du code
        public AddOperationViewModel() : base() {
            //Tricount = tricount;
            Tricount = Context.Tricounts.Find(4);// pour les tests
            // Une fois liée au reste du code à décommenté
           // _currentUser = App.CurrentUser.FullName;
           _currentUser = Context.Users.Find(2);
           AddCommand = new RelayCommand(AddAction,
               () => _title != null && _amount != null && _currentUser != null && !HasErrors);
        }
        
        // Commandes
        public ICommand AddCommand { get; set; }
        
        // Attributes
        private string _title;
        private string _amount;
        private User _currentUser;
        private Tricount _tricount;
        private bool _ischecked;

        // Properties

        public bool IsChecked {
            get => _ischecked;
            set => SetProperty(ref _ischecked, value, () => Validate());
        }
        
        public DateTime StartDate {
            get => Tricount.CreatedAt;
            set {
                DateTime tricountCreatedAt = Tricount.CreatedAt;
                SetProperty(ref tricountCreatedAt, value);
            }
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
            get => _title;
            set => SetProperty(ref _title, value, () => Validate());
        }
        
        // Méthodes Commandes
        
        // Add
        private void AddAction() {
            if (Validate()) {
                var operation = new Operation(Title, Tricount.Id, double.Parse(Amount), DateTime.Today, CurrentUser.UserId );
                NotifyColleagues(App.Messages.MSG_ADD_OPERATION, operation);
            }
        }

        // Méthode de validation
        public override bool Validate() {
            ClearErrors();
            
            if (string.IsNullOrEmpty(Title))
                AddError(nameof(Title), "required");
            else {
                if (IsValidAmount(Amount) != "" ) {
                    AddError(nameof(Amount), "Must contain only numbers");
                }
                else if (double.Parse(Amount) < 0.01) {
                    AddError(nameof(Amount), "minimum 1 cent");
                }

                if (IsChecked == false) {
                    AddError(nameof(IsChecked), "Checked");
                }
            }
            
            return !HasErrors;
        }
        
        private string IsValidAmount(string amount) {
            // Regex to match a string that contains only digits
            var onlyNumbers = new System.Text.RegularExpressions.Regex(@"^\d+(\.\d+)?$");

    
            // Check if the password matches the regex
            if (!onlyNumbers.IsMatch(amount))
                return "Must contain only numbers";
    
            return "";
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
        
    }
}