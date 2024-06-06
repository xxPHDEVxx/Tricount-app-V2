using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.Windows.Controls;
using System.Windows.Input;

namespace prbd_2324_a06.View
{
    public partial class ViewTricountView : UserControlBase
    {
        private readonly ViewTricountViewModel _vm;

        public ViewTricountView(Tricount tricount) {
            InitializeComponent();
            DataContext = _vm = new ViewTricountViewModel(tricount);
            Register<Operation>(App.Messages.MSG_OPEN_OPERATION,
                operation => OpenOperation(operation, false));
            Register<Operation>(App.Messages.MSG_OPEN_NEW_OPERATION,
                operation => OpenOperation(operation, true));
        }

        private void OpenOperation(Operation operation, bool isNew) {
            if (operation != null) {
                if (isNew) {
                    var window = new AddOperationView(operation);
                    window.Show();
                } else {
                    var window = new EditOperationView(operation);
                    window.Show();
                }
            }
        }
    }
}