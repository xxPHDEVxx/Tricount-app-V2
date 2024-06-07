using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows;

namespace prbd_2324_a06.View
{
    public partial class MainView : WindowBase
    {
        private List<Window> _otherWindows = new List<Window>();

        public MainView()
        {
            InitializeComponent();

            // Enregistrement des messages pour l'affichage des différentes vues
            Register<User>(App.Messages.MSG_DISPLAY_SIGN_UP, _ => DisplaySignUpView());
            Register<Tricount>(App.Messages.MSG_NEW_TRICOUNT, tricount => DoDisplayTricount(tricount, true));
            Register<Tricount>(App.Messages.MSG_DISPLAY_TRICOUNT, tricount => DoDisplayViewTricount(tricount));
            Register<Operation>(App.Messages.MSG_OPEN_OPERATION, operation => OpenOperation(operation, false));
            Register<Operation>(App.Messages.MSG_OPEN_NEW_OPERATION, operation => OpenOperation(operation, true));
        }

        // Affichage de la fenêtre de sign up
        private void DisplaySignUpView()
        {
            var signUpWindow = new SignUpView();
            Close(); // Ferme la fenêtre principale
            signUpWindow.Show(); // Affiche la fenêtre de sign up
        }

        // Gestionnaire d'événement pour le clic sur le bouton de déconnexion
        private void Logout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }

        // Gestionnaire d'événement pour le clic sur le bouton de réinitialisation
        private void Reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotifyColleagues(App.Messages.MSG_RESET);
        }

        // Affichage d'un tricount dans un nouvel onglet
        private void DoDisplayTricount(Tricount tricount, bool isNew)
        {
            if (tricount != null)
                OpenTab(isNew ? "<New Tricount>" : tricount.Title, tricount.Title, () => new TricountDetailView(tricount, isNew));
        }

        // Affichage d'un tricount dans une vue de consultation
        private void DoDisplayViewTricount(Tricount tricount)
        {
            if (tricount != null)
                OpenTab(tricount.Title, tricount.Title, () => new ViewTricountView(tricount));
        }

        // Ouverture d'un nouvel onglet avec une vue spécifique
        private void OpenTab(string header, string tag, Func<UserControlBase> createView)
        {
            var tab = tabControl.FindByTag(tag);
            if (tab == null)
                tabControl.Add(createView(), header, tag);
            else
                tabControl.SetFocus(tab);
        }

        // Ouverture d'une opération dans une fenêtre modale
        private void OpenOperation(Operation operation, bool isNew)
        {
            if (operation != null)
            {
                if (isNew)
                {
                    var window = new AddOperationView(operation);
                    window.Show();
                }
                else
                {
                    var window = new EditOperationView(operation);
                    window.Show();
                }
            }
        }
        
        
    }
}
