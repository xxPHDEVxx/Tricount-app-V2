using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel
{
    public class ViewTricountViewModel : ViewModelCommon
    {
        public ViewTricountViewModel(Tricount tricount) : base() {
            Tricount = tricount;
            OnRefreshData();
            ClearFilter = new RelayCommand(() => Filter = "");
            DisplayOperation = new RelayCommand<TricountCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_DISPLAY_OPERATIONS, vm.Tricount);
            });
            OpenEditOperation = new RelayCommand<OperationCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_OPEN_OPERATION, vm.Operation);
            });
            OpenNewOperation = new RelayCommand<OperationCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_OPEN_NEW_OPERATION, new Operation(tricount.Id));
            });
            Register<Operation>(App.Messages.MSG_OPERATION_CHANGED, operation => OnRefreshData());

        }

        private ObservableCollection<OperationCardViewModel> _operations;
        private Tricount _tricount;
        private string _filter;

        // Properties

        public ObservableCollection<OperationCardViewModel> Operations {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

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

        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        public DateTime CreatedAt {
            get => Tricount.CreatedAt;
            init => SetProperty(Tricount.CreatedAt, value, Tricount
                , (t, d) => { });
        }

        // Commandes
        public ICommand ClearFilter { get; set; }
        public ICommand DisplayOperation { get; set; }
        public ICommand OpenNewOperation { get; set; }
        public ICommand OpenEditOperation { get; set; }


        // Méthodes 

        // Permet le Refresh
        protected override void OnRefreshData() {
            if (Tricount == null) return;

            IQueryable<Operation> operations = Tricount.GetOperations();

            Operations = new ObservableCollection<OperationCardViewModel>(operations.Select(o =>
                new OperationCardViewModel(o)));
        }
    }
}