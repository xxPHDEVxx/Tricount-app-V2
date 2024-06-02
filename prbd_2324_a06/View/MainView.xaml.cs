using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.View;

public partial class MainView : WindowBase
{
    public MainView() {
        InitializeComponent();
        
    }
    
    private void Logout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }
    
}