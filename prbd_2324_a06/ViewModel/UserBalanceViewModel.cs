using prbd_2324_a06.Model;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Media;

namespace prbd_2324_a06.ViewModel
{
    public class UserBalanceViewModel : ViewModelCommon
    {
        public UserBalanceViewModel(User user, Tricount tricount) {
            User = user;
            Tricount = tricount;
            UpdateStyle();
        }

        private User _user;
        private Tricount _tricount;

        public User User {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        public string Balance {
            get => $"{User.GetMyBalance(Tricount):F2}";
            set => SetProperty($"{User.GetMyBalance(Tricount):F2}", value
                , Tricount, (t, b) => { });
        }

        private Brush _backgroundColor;

        public Brush BackgroundColor {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        private int _columnB;

        public int ColumnB {
            get => _columnB;
            set => SetProperty(ref _columnB, value);
        }

        private int _columnA;

        public int ColumnA {
            get => _columnA;
            set => SetProperty(ref _columnA, value);
        }

        private HorizontalAlignment _sideA;

        public HorizontalAlignment SideA {
            get => _sideA;
            set => SetProperty(ref _sideA, value);
        }

        private HorizontalAlignment _sideB;

        public HorizontalAlignment SideB {
            get => _sideB;
            set => SetProperty(ref _sideB, value);
        }

        private void UpdateStyle() {
            if (int.Parse(Balance) >= 0) {
                SideA = HorizontalAlignment.Right;
                SideB = HorizontalAlignment.Left;
                ColumnA = 0;
                ColumnB = 1;
                BackgroundColor = Brushes.LightGreen;
            } else {
                SideA = HorizontalAlignment.Left;
                SideB = HorizontalAlignment.Right;
                ColumnA = 1;
                ColumnB = 0;
                BackgroundColor = Brushes.Salmon;
            }
        }
    }
}