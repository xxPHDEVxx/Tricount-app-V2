using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class SignUpViewModel : ViewModelCommon
    {
        public ICommand SignUpCommand { get; set; }

        private string _mail;
        private string _name;
        private string _password;
        private string _checkPaswword;

        public string Mail {
            get => _mail;
            set => SetProperty(ref _mail, value, () => Validate());
        }

        public string Name {
            get => _name;
            set => SetProperty(ref _name, value, () => Validate());
        }

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public string CheckPassword {
            get => _checkPaswword;
            set => SetProperty(ref _checkPaswword, value, () => Validate());
        }

        public SignUpViewModel() : base() {
            SignUpCommand = new RelayCommand(SignUpAction,
                () => _mail != null && _password != null
                                    && _checkPaswword != null && !HasErrors);
        }
        
        private void SignUpAction() {
            if (Validate()) {
                var user = new User(Mail, Password, Name, 0);
                Context.Users.Add(user);
                Context.SaveChanges();
                NotifyColleagues(App.Messages.MSG_SIGN_UP, user);
            }
        }


        public override bool Validate() {
            ClearErrors();

            var user = Context.Users.FirstOrDefault(u => u.Mail == Mail);

            if (string.IsNullOrEmpty(Mail))
                AddError(nameof(Mail), "required");
            else if (!IsValidEmail(Mail))
                AddError(nameof(Mail), "invalid email format");
            else if (IsEmailAlreadyExist(Mail))
                AddError(nameof(Mail), "email already exists");
            else {
                if (string.IsNullOrEmpty(Name))
                    AddError(nameof(Name), "required");
                else if (Name.Length < 3)
                    AddError(nameof(Name), "length must be >= 3");
                else if (IsNameAlreadyExist(Name))
                    AddError(nameof(Mail), "email already exists");
                else {
                    if (string.IsNullOrEmpty(Password))
                        AddError(nameof(Password), "required");
                    else if (Password.Length < 8)
                        AddError(nameof(Password), "length must be >= 8");
                    else if (IsValidPassword(Password) != "")
                        AddError(nameof(Password), IsValidPassword(Password));
                    else {
                        if (Password != CheckPassword) {
                            AddError(nameof(CheckPassword), "Should be same as Password");
                        }
                    }
                }
            }

            return !HasErrors;
        }

        private string IsValidPassword(string password) {
            // Vérifie si le mot de passe contient au moins un chiffre, une lettre majuscule et un caractère non alphanumérique
            var hasNumber = new System.Text.RegularExpressions.Regex(@"[0-9]+");
            var hasUpperChar = new System.Text.RegularExpressions.Regex(@"[A-Z]+");
            var hasSymbols = new System.Text.RegularExpressions.Regex(@"[!@#$%^&*(),.?""{}|<>-]");
            if (!(hasNumber.IsMatch(password)))
                return "Must contain at least a number";
            else if (!(hasUpperChar.IsMatch(password)))
                return "Must contain at least an upper case";
            else if (!(hasSymbols.IsMatch(password)))
                return "Must contain at least a symbol";
            else return "";
        }

        private bool IsValidEmail(string email) {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            } catch {
                return false;
            }
        }

        private bool IsEmailAlreadyExist(string email) {
            var member = Context.Users.FirstOrDefault(u => u.Mail == email);
            return member != null;
        }

        private bool IsNameAlreadyExist(string name) {
            var member = Context.Users.FirstOrDefault(u => u.FullName == name);
            return member != null;
        }
    }
}