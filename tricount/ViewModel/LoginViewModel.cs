using Microsoft.EntityFrameworkCore;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Windows.Interop;

namespace prbd_2324_a06.ViewModel
{
    public class LoginViewModel : ViewModelCommon
    {
        public ICommand LoginCommand { get; set; }
        public ICommand LoginAsCommand { get; set; }
        public ICommand GoToSignUpCommand { get; set; }

        // attributs et propriétés
        private string _mail;
        private User[] _defaultUser;
        private Role[] _defaultUserRole;
        private string _password;
        
        public string Mail {
            get => _mail;
            set => SetProperty(ref _mail, value, () => Validate());
        }
        
        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public User[] DefaultUser {
            get => _defaultUser;
            set => SetProperty(ref _defaultUser, value);
        }

        public Role[] DefaultUserRole {
            get => _defaultUserRole;
            set => SetProperty(ref _defaultUserRole, value);
        }

        /**
         * Constructeur
         * -> initialisation données pour affichage sur view
         * -> Mise en place des commandes pour navigation ( Login, Sign up, ...)
         */
        public LoginViewModel() : base() {
            DefaultUserRole = new Role[4];
            DefaultUser = new User[4];
            int user = 1;
            for (int i = 0; i < 4; i++) {
                if (user == 4) {
                    user++;
                }

                DefaultUser[i] = Context.Users.FirstOrDefault(u => u.UserId == user);
                DefaultUserRole[i] = DefaultUser[i].Role;
                user++;
            }

            LoginCommand = new RelayCommand(LoginAction,
                () => _mail != null && _password != null && !HasErrors);
            LoginAsCommand = new RelayCommand<User>(LoginAsUser);
            GoToSignUpCommand = new RelayCommand(() => {
                NotifyColleagues(App.Messages.MSG_DISPLAY_SIGN_UP, new User());
            });
        }

        // Connexion
        private void LoginAction() {
            if (Validate()) {
                var user = Context.Users.FirstOrDefault(u => u.Mail == Mail);
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }

        // Connexion par défaut
        private void LoginAsUser(User user) {
            if (user != null) {
                // Logique de connexion en utilisant les informations de l'utilisateur
                var email = user.Mail;
                var password = user.HashedPassword;
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }

        // Validations
        public override bool Validate() {
            ClearErrors();

            var user = Context.Users.FirstOrDefault(u => u.Mail == Mail);

            if (string.IsNullOrEmpty(Mail))
                AddError(nameof(Mail), "required");
            else if (!IsValidEmail(Mail))
                AddError(nameof(Mail), "invalid email format");
            else if (user == null)
                AddError(nameof(Mail), "does not exist");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 8)
                    AddError(nameof(Password), "length must be >= 8");
                else if (IsValidPassword(Password) != "")
                    AddError(nameof(Password), IsValidPassword(Password));
                else if (user.HashedPassword != Password)
                    AddError(nameof(Password), "wrong password");
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

        protected override void OnRefreshData() {
        }
    }
}