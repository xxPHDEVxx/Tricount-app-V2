using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class LoginViewModel : ViewModelBase<User, PridContext>
    {
        public ICommand LoginCommand { get; set; }

        private string _mail;

        private Enum message;

        public string Mail {
            get => _mail;
            set => SetProperty(ref _mail, value, () => Validate());
        }

        private string _password;

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public LoginViewModel() {
            LoginCommand = new RelayCommand(LoginAction,
                () => _mail != null && _password != null && !HasErrors);
        }

        private void LoginAction() {
            if (Validate()) {
                var user = Context.Users.Find(Mail);
                NotifyColleagues(message, user);
            }
        }

        protected override void OnRefreshData() {
        }

        public override bool Validate() {
            ClearErrors();

            var user = Context.Users.Find(Mail);

            if (string.IsNullOrEmpty(Mail))
                AddError(nameof(Mail), "required");
            else if (!IsValidEmail(Mail))
                AddError(nameof(Mail), "invalid email format");
            else if (IsEmailAlreadyExist(Mail))
                AddError(nameof(Mail), "email already exists");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 3)
                    AddError(nameof(Password), "length must be >= 3");
                else if (user.HashedPassword != Password)
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

        private bool IsEmailAlreadyExist(string email) {
            var user = Context.Users.FirstOrDefault(u => u.Mail == email);
            return user != null;
        }
    }
}