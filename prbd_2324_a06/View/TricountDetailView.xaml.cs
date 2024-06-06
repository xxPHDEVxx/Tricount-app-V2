using PRBD_Framework;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;


namespace prbd_2324_a06.View
{
    /// <summary>
    /// Logique d'interaction pour AddTricountView.xaml
    /// </summary>
    public partial class TricountDetailView : UserControlBase
    {
        private readonly TricountDetailViewModel _vm;

        public TricountDetailView(Tricount tricount, bool isNew) {
            InitializeComponent();
            DataContext = _vm = new TricountDetailViewModel(tricount, isNew);

        }
    }
}
