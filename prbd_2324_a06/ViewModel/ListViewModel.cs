using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class ListViewModel : ViewModelCommon
{
    private ObservableCollection<TricountCardViewModel> _tricounts;

    public ObservableCollection<TricountCardViewModel> Tricounts {
        get => _tricounts;
        set => SetProperty(ref _tricounts, value);
    }

    public ICommand ClearFilter { get; set; }
    public ICommand DisplayTricount { get; set; }

    public ICommand NewTricount { get; set; }

    //observable de card tricount
    private string _filter;

    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, OnRefreshData);
    }

    public ListViewModel() : base() {
        OnRefreshData();
        ClearFilter = new RelayCommand(() => Filter = "");
        NewTricount = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_OPEN_TRICOUNT, new Tricount());
        });
        DisplayTricount = new RelayCommand<TricountCardViewModel>(vm => {
            NotifyColleagues(App.Messages.MSG_DISPLAY_TRICOUNT, vm.Tricount);
        });
        Console.WriteLine(CurrentUser.Role);
        Register<Tricount>(App.Messages.MSG_TRICOUNT_CHANGED, tricount => OnRefreshData());
        Register<Tricount>(App.Messages.MSG_OPERATION_TRICOUNT_CHANGED, tricount => OnRefreshData());
        Register( App.Messages.MSG_RESET, () => OnRefreshData());
    }

    protected override void OnRefreshData() {

        if (CurrentUser.Role is Role.Administrator) {
            IQueryable<Tricount> tricounts = string.IsNullOrEmpty(Filter) ? CurrentUser.GetAll()
                : CurrentUser.GetAllFiltered(Filter);
        Tricounts = new ObservableCollection<TricountCardViewModel>(tricounts.Select(t =>
            new TricountCardViewModel(t)));
        } else {

        IQueryable<Tricount> tricounts = string.IsNullOrEmpty(Filter) ? CurrentUser.GetTricounts().Union(CurrentUser.GetParticipatedTricounts()) 
            : CurrentUser.GetFiltered(Filter);

        Tricounts = new ObservableCollection<TricountCardViewModel>(tricounts.Select(t =>
            new TricountCardViewModel(t)));
        }
    }
}