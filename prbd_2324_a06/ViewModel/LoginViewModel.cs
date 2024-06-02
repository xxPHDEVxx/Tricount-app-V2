using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class LoginViewModel : ViewModelCommon
    {
        public ICommand LoginCommand { get; set; }

        private string _mail;

        private Enum _message;

        public string Mail {
            get => _mail;
            set => SetProperty(ref _mail, value, () => Validate());
        }

        private string _password;

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction,
                () => _mail != null && _password != null && !HasErrors);
        }

        private void LoginAction() {
            if (Validate()) {
                var user = Context.Users.FirstOrDefault(u => u.Mail == Mail);
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