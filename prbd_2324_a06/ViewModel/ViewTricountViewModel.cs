using prbd_2324_a06.Model;
using System.Runtime.InteropServices.JavaScript;

namespace prbd_2324_a06.ViewModel
{
    public class ViewTricountViewModel : ViewModelCommon
    {
        public ViewTricountViewModel(Tricount tricount) : base() {
            Tricount = tricount;
        }

        private Tricount _tricount;
        private DateTime _createdAt;

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public string Title {
            get => Tricount.Title;
            set => SetProperty(Tricount.Title, value, Tricount, (tr, t) => {
            });
        }
        public string Description {
            get => Tricount.Description;
            set => SetProperty(Tricount.Description, value, Tricount, (tr, d) => {
            });
        }

        public DateTime CreatedAt {
            get => Tricount.CreatedAt;
            init => SetProperty(Tricount.CreatedAt, value, Tricount
                , (t, d) => { });
        }
    }
}