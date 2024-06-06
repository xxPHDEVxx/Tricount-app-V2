using prbd_2324_a06.Model;

namespace prbd_2324_a06.ViewModel
{
    public class ViewTricountViewModel : ViewModelCommon
    {
        public ViewTricountViewModel(Tricount tricount) : base() {
            Tricount = tricount;
        }

        private Tricount _tricount;

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }
    }
}