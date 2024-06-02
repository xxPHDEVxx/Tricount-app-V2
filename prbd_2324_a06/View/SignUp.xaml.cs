using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_a06.View
{
    public partial class SignUp : WindowBase
    {
        public SignUp() {
            InitializeComponent();
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}