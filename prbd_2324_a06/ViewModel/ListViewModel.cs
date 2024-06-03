using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

    public class ListViewModel : ViewModelCommon {
        private ObservableCollection<TricountCardViewModel> _tricounts;

        public ObservableCollection<TricountCardViewModel> Tricounts {
            get => _tricounts;
            set => SetProperty(ref _tricounts, value);
        }
        public ICommand ClearFilter { get; set; }

        public ICommand NewTricount {  get; set; }

            //observable de card tricount
            private string _filter;
            public string Filter {
                get => _filter;
                set => SetProperty(ref _filter, value, OnRefreshData);
            }

            public ListViewModel() : base() {

                OnRefreshData();
                ClearFilter = new RelayCommand(() => Filter = "");

    }

    protected override void OnRefreshData() {
            var UserId = CurrentUser.UserId;
        
            IQueryable<Tricount> tricounts = string.IsNullOrEmpty(Filter) ? Tricount.GetAll(UserId) : Tricount.GetFiltered(Filter, UserId);

            Tricounts = new ObservableCollection<TricountCardViewModel>(tricounts.Select(t => new TricountCardViewModel(t)));
            }
    }

