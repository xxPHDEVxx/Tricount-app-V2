using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.View;

public partial class MainView : WindowBase
{
    public MainView() {
        InitializeComponent();
        
    }
    // pour les fenêtre du main
    /*private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }*/
    
}