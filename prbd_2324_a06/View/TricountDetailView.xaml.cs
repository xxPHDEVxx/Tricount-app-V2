using PRBD_Framework;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using NumericUpDownLib;
using System.Windows.Controls;
using System.Windows;


namespace prbd_2324_a06.View
{
    /// <summary>
    /// Logique d'interaction pour TricountDetailView.xaml
    /// </summary>
    public partial class TricountDetailView : UserControlBase
    {
        private readonly TricountDetailViewModel _vm;

        public TricountDetailView(Tricount tricount, bool isNew) {
            DataContext = _vm = new TricountDetailViewModel(tricount, isNew);
            InitializeComponent();
            InitializeComboBox();

        }

        private void InitializeComboBox() {
            // fetching users from the database
            List<User> users = new List<User>();

            foreach (var user in users) {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Create Textbox
                TextBox textBox = new TextBox { };
                Grid.SetColumn(textBox, 0);
                userGrid.Children.Add(textBox);

                // Create NumericUpDown
                Button delete = new Button {
                  
                };
                Grid.SetColumn(delete, 1);
                userGrid.Children.Add(delete);



                // Add the userGrid to the ParticipantsPanel
                PanelParticipants.Children.Add(userGrid);

            }
        }


    }
}
