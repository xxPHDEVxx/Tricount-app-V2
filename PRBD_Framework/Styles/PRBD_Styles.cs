using System.Windows.Controls;

namespace PRBD_Framework; 

public partial class PRBD_Styles {
    private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
        ((TextBox)sender).SelectAll();
    }

    private void PasswordBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
        ((PasswordBox)sender).SelectAll();
    }
}