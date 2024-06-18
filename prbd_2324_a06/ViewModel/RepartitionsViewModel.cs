using prbd_2324_a06.Model;

namespace prbd_2324_a06.ViewModel
{
    public class RepartitionsViewModel : ViewModelCommon
    {
        public RepartitionsViewModel(Repartition repartition, Operation operation, string amount) {
            Repartition = repartition;
            Operation = operation;
            Amount = amount;
        }

        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => {
                CalculAmount();
            });
        }

        public string MyAmount {
            get => _myAmount;
            set => SetProperty(ref _myAmount, value, () => {
            });
        }

        private Repartition _repartition;
        private Operation _operation;
        private string _amount;
        private string _myAmount;

        public Repartition Repartition {
            get => _repartition;
            set => SetProperty(ref _repartition, value);
        }

        public Operation Operation {
            get => Operation.GetOperationById(Repartition.OperationId);
            set => SetProperty(ref _operation, value);
        }

        public User User {
            get => Repartition.User ?? User.GetUserById(Repartition.UserId);
            set => SetProperty(Repartition.User, value, User, (r, u) => { });
        }

        public int Weight {
            get => Repartition.Weight;
            set => SetProperty(Repartition.Weight, value, Repartition, (r, w) => { CalculAmount();});
        }

        public void CalculAmount() {
            int totalWeight = 0;
            if (Operation.Repartitions != null) {
                // Calcul du total des poids
                foreach (var r in Operation.Repartitions) {
                    totalWeight += r.Weight;
                }

                // insertion montants dans textblock
                double part = totalWeight < 1
                    ? double.Parse(Amount) * totalWeight
                    : double.Parse(Amount) / totalWeight;

                MyAmount = $"{part * Weight:F2} €";
            }
        }
    }
}