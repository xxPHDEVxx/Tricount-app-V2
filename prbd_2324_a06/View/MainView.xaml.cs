using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.View;

public partial class MainView : WindowBase
{
    public MainView() {
        InitializeComponent();
            Register<Tricount>(App.Messages.MSG_NEW_TRICOUNT,
            tricount => DoDisplayTricount(tricount, true));
    }

    private void Logout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }
    
    private void Reset_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_RESET);
    }

    private void DoDisplayTricount(Tricount tricount, bool isNew) {
        if (tricount != null)
            OpenTab(isNew ? "<New Tricount>" : tricount.Title, tricount.Title, () => new TricountDetailView(tricount, isNew));
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }
}