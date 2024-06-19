using prbd_2324_a06.Model;

namespace prbd_2324_a06.ViewModel
{
    public class RepartitionsViewModel : ViewModelCommon
    {
        public RepartitionsViewModel(Repartition repartition, OperationViewModel vm) {
            Repartition = repartition;
            Vm = vm;
            CalculAmount();
        }

        private Repartition _repartition;
        private string _myAmount;
        private OperationViewModel _vm;
        private int _weight;

        public OperationViewModel Vm {
            get => _vm;
            init => SetProperty(ref _vm, value);
        }

        public Repartition Repartition {
            get => _repartition;
            set => SetProperty(ref _repartition, value);
        }

        public User User {
            get => Repartition.User ?? User.GetUserById(Repartition.UserId);
            set => SetProperty(Repartition.User, value, User, (r, u) => { });
        }

        public int Weight {
            get => Repartition.Weight;
            set {
                if (SetProperty(ref _weight, value)) {
                    Repartition.Weight = value;
                    CalculAmount();
                }
            }
        }
        
        public string MyAmount {
            get => _myAmount;
            set => SetProperty(ref _myAmount, value);
        }

        public bool IsChecked {
            get => Weight > 0;
            set => SetProperty(Weight > 0, value, Repartition, (r, w) => CalculAmount());
        }

        public void CalculAmount() {
            int totalWeight = 0;
            if (Vm.Operation.Repartitions != null) {
                // Calcul du total des poids
                foreach (var r in Vm.Operation.Repartitions) {
                    totalWeight += r.Weight;
                }

                // Calcul du montant à afficher
                double part = totalWeight < 1
                    ? double.Parse(Vm.Amount) * totalWeight
                    : double.Parse(Vm.Amount) / totalWeight;

                MyAmount = $"{part * Weight:F2} €";
            }
        }
    }
}
