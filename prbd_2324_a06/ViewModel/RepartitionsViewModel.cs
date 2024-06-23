using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.ViewModel
{
    public class RepartitionsViewModel : ViewModelCommon
    {
        public RepartitionsViewModel(Repartition repartition, OperationViewModel vm) {
            Repartition = repartition;
            Vm = vm;
            Weight = Repartition.Weight;
            IsChecked = Repartition.Weight != 0;
            CalculAmount();
            Register(App.Messages.AMOUNT_CHANGED, CalculAmount);
        }

        private Repartition _repartition;
        private string _myAmount;
        private OperationViewModel _vm;
        private int _weight;
        private bool _isChecked;

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
            get => _weight;
            set => SetProperty(ref _weight, value, () => {
                IsChecked = Weight != 0;
                foreach (var repartition in Vm.Repartitions) {
                    repartition.CalculAmount();
                }
            });
        }

        public string MyAmount {
            get => _myAmount;
            set => SetProperty(ref _myAmount, value);
        }

        public bool IsChecked {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value, () => {
                Vm.Validate();
                Weight = IsChecked ? 1 : 0;
            });
        }

        public void CalculAmount() {
            int totalWeight = 0;
            if (Vm.Operation.Repartitions != null) {
                // Calcul du total des poids
                foreach (var r in Vm.Repartitions) {
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