using Microsoft.EntityFrameworkCore;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Windows.Interop;

namespace prbd_2324_a06.ViewModel
{
    public class LoginViewModel : ViewModelCommon
    {
        public ICommand LoginCommand { get; set; }
        public ICommand LoginAsCommand { get; set; }

        private string _mail;

        public string Mail {
            get => _mail;
            set => SetProperty(ref _mail, value, () => Validate());
        }

        private string _password;

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        private User[] _defaultUser;
        private string[] _defaultUserName;

        public User[] DefaultUser {
            get => _defaultUser;
            set => SetProperty(ref _defaultUser, value);
        }

        public string[] DefaultUserName {
            get => _defaultUserName;
            set => SetProperty(ref _defaultUserName, value);
        }

        public LoginViewModel() : base() {
            DefaultUser = new User[4];
            DefaultUserName = new string[4];
            int user = 1;
            for (int i = 0; i < 4; i++) {
                if (user == 4) {
                    user++;
                }
                DefaultUser[i] = Context.Users.FirstOrDefault(u => u.UserId == user);
                DefaultUserName[i] = DefaultUser[i].FullName;
                user++;
            }

            LoginCommand = new RelayCommand(LoginAction,
                () => _mail != null && _password != null && !HasErrors);
            LoginAsCommand = new RelayCommand<User>(LoginAsUser);
        }

        private void LoginAction() {
            if (Validate()) {
                var user = Context.Users.FirstOrDefault(u => u.Mail == Mail);
                NotifyColleagues(ApplicationBaseMessages.MSG_LOGIN, user);
            }
        }

        private void LoginAsUser(User user) {
            if (user != null) {
                // Logique de connexion en utilisant les informations de l'utilisateur
                var email = user.Mail;
                var password = user.HashedPassword;
                NotifyColleagues(ApplicationBaseMessages.MSG_LOGIN, user);
            }
        }

        public override bool Validate() {
            ClearErrors();

            var user = Context.Users.FirstOrDefault(u => u.Mail == Mail);

            if (string.IsNullOrEmpty(Mail))
                AddError(nameof(Mail), "required");
            else if (!IsValidEmail(Mail))
                AddError(nameof(Mail), "invalid email format");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 3)
                    AddError(nameof(Password), "length must be >= 3");
                else if (user != null && user.HashedPassword != Password)
                    AddError(nameof(Password), "wrong password");
            }

            return !HasErrors;
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