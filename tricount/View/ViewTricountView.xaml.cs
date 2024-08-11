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
        }
    }
}