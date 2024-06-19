using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_a06.View
{
    public partial class MainView : WindowBase
    {
        public MainView() {
            InitializeComponent();

            // Enregistrement des messages pour l'affichage des différentes vues
            Register<Tricount>(App.Messages.MSG_OPEN_TRICOUNT, tricount => DoDisplayTricount(tricount, true));
            Register<Tricount>(App.Messages.MSG_EDIT_TRICOUNT, tricount => DoDisplayTricount(tricount, false));
            Register<Tricount>(App.Messages.MSG_DISPLAY_TRICOUNT, tricount => DoDisplayViewTricount(tricount));
            Register<Operation>(App.Messages.MSG_OPEN_OPERATION, operation => OpenOperationAction(operation));
            Register<Operation>(App.Messages.MSG_OPEN_NEW_OPERATION, operation => OpenOperationAction(operation));
            Register<Tricount>(App.Messages.MSG_CLOSE_TAB, tricount => DoCloseTab(tricount));
            Register<Tricount>(App.Messages.MSG_TRICOUNT_CHANGED, tricount => DoCloseTab(tricount));
            Register(App.Messages.MSG_DELETED, DeleteAction);

            Register<Tricount>(App.Messages.MSG_TITLE_CHANGED,
            tricount => DoRenameTab(string.IsNullOrEmpty(tricount.Title) ? "<New Tricount>" : tricount.Title));
        }


        // Gestionnaire d'événement pour le clic sur le bouton de déconnexion
        private void Logout_Click(object sender, System.Windows.RoutedEventArgs e) {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }

        // Gestionnaire d'événement pour le clic sur le bouton de réinitialisation
        private void Reset_Click(object sender, System.Windows.RoutedEventArgs e) {
            NotifyColleagues(App.Messages.MSG_RESET);
        }

        // Affichage d'un tricount dans un nouvel onglet
        private void DoDisplayTricount(Tricount tricount, bool isNew) {
            if (tricount != null) {
                if (isNew) {
                    OpenTab("<New Tricount>", "<New Tricount>", () => new TricountDetailView(tricount, true));
                } else {
                    OpenTab(tricount.Title, tricount.Title, () => new TricountDetailView(tricount, false));
                }
            }
        }

        // Affichage d'un tricount dans une vue de consultation
        private void DoDisplayViewTricount(Tricount tricount) {
            if (tricount != null)
                OpenTab(tricount.Title, tricount.Title, () => new ViewTricountView(tricount));
        }

        // Ouverture d'un nouvel onglet avec une vue spécifique
        private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
            var tab = tabControl.FindByTag(tag);
            if (tab == null)
                tabControl.Add(createView(), header, tag);
            else
                tabControl.SetFocus(tab);
        }
        private void OpenOperationAction(Operation operation) {
            ApplicationRoot.ShowDialog<OperationViewModel, Operation, PridContext>(operation);
        }

        private void DeleteAction() {
            MessageBox.Show("Delete was successful", "Delete Confirmation");
        }

        private void DoCloseTab(Tricount tricount) {
            if (tabControl.FindByTag((tricount.Title)) == null)
                tabControl.CloseByTag("<New Tricount>");
            else
                tabControl.CloseByTag((tricount.Title));
        }

        private void DoRenameTab(string header) {
            if (tabControl.SelectedItem is TabItem tab) {
                MyTabControl.RenameTab(tab, header);
                tab.Tag = header;
            }
        }
        
        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            tabControl.Dispose();
        }
    }
}