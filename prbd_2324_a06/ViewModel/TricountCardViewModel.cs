using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.ViewModel
{
    class TricountCardViewModel : ViewModelCommon
    {

        private readonly Tricount _tricount;

        public TricountCardViewModel(Tricount tricount) {
            _tricount = tricount;
        }
    }
}
