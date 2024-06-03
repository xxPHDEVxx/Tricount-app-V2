using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class ListViewModel : ViewModelCommon {
        public ICommand NewTricount {  get; set; }

        //observable de card tricount
        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        public ListViewModel() { }
    }
}
