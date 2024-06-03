using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class ListViewModel : ViewModelCommon {
        public ICommand NewTricount {  get; set; }
        
        //observable de card tricount
        //filter

        public ListViewModel() { }
    }
}
