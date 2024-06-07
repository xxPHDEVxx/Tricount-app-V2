using PRBD_Framework;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using NumericUpDownLib;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Tokens;


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
            newParticipants();
        }

        private void InitializeComboBox() {
            PanelParticipants.Children.Clear();
            // fetching users from the database
            ObservableCollection<string> users = _vm.Participants;
            if (!users.IsNullOrEmpty()) {
                
            foreach (var user in users) {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create Textbox
                TextBox textBox = new TextBox { Text = user};
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

        private void newParticipants() {
            btnAdd.Click += (sender, e) => {
                InitializeComboBox();
            };
        }


    }
}
