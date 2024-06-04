using Microsoft.Extensions.Options;
using prbd_2324_a06.Model;

namespace prbd_2324_a06.ViewModel
{
    public class AddOperationViewModel : ViewModelCommon
    {
        // ajouter en parametre Tricount pour lier au reste du code
        public AddOperationViewModel() : base() {
            //Tricount = tricount;
            Tricount = Context.Tricounts.Find(1);// pour les tests
            // Une fois liée au reste du code à décommenté
           // _currentUser = App.CurrentUser.FullName;
           _currentUser = "Benoît";
        }
        
        // Attributes
        private string _title;
        private string _amount;
        private string _currentUser;
        private Tricount _tricount;

        // Properties

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
        public string CurrentUser {
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
    }
}