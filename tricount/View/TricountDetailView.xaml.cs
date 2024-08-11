using PRBD_Framework;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;


namespace prbd_2324_a06.View
{
    /// <summary>
    /// Logique d'interaction pour TricountDetailView.xaml
    /// </summary>
    public partial class TricountDetailView : UserControlBase
    {
        private readonly TricountDetailViewModel _vm;

        public TricountDetailView(Tricount tricount, bool isNew) {
            DataContext = _vm = new TricountDetailViewModel(tricount, isNew);
            InitializeComponent();
        }

    }
}
