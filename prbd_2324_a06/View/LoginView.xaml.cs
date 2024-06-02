using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_a06.View
{
    public partial class LoginView : WindowBase
    {
        public LoginView() {
            InitializeComponent();
            
            // Affichage fenêtre sign up
            Register<User>(App.Messages.MSG_DISPLAY_SIGN_UP, _ => DisplaySignUpView());
        }
        
        // Affichage fenêtre Sign up
        private void DisplaySignUpView() {
            var signUpWindow = new SignUpView();
            Close();
            signUpWindow.Show();
        }
        
        // Bouton Cancel
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

    }
}