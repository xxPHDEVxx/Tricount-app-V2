using NumericUpDownLib;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_a06.View
{
    public partial class AddOperationView : WindowBase
    {
        public AddOperationView() {
            InitializeComponent();
            InitializeCheckBox();
            InitializeCombobox();
        }

        // Initialise checkBox's template with the tricount's participants
        private void InitializeCheckBox() {
            // fetching users from the database
            List<User> users = GetUsersFromDatabase();

            foreach (var user in users) {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create CheckBox
                CheckBox checkBox = new CheckBox
                    { Content = user.FullName, Margin = new Thickness(5), IsChecked = true };
                Grid.SetColumn(checkBox, 0);
                userGrid.Children.Add(checkBox);

                // Create NumericUpDown
                NumericUpDown numericUpDown = new NumericUpDown {
                    Width = 60,
                    Value = 1,
                    MinValue = 0,
                    Margin = new Thickness(5)
                };
                numericUpDown.ValueChanged += (sender, e) => UpdateCheckBoxState(checkBox, numericUpDown);
                Grid.SetColumn(numericUpDown, 1);
                userGrid.Children.Add(numericUpDown);

                // Create TextBlock
                TextBlock textBlock = new TextBlock
                    { Text = "0,00 €", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5) };
                Grid.SetColumn(textBlock, 2);
                userGrid.Children.Add(textBlock);


                // Add the userGrid to the ParticipantsPanel
                ParticipantsPanel.Children.Add(userGrid);

                // Gestionnaire d'événements pour la CheckBox
                checkBox.Checked += (sender, e) => UpdateNumericUpDownState(checkBox, numericUpDown);
                checkBox.Unchecked += (sender, e) => UpdateNumericUpDownState(checkBox, numericUpDown);
            }
        }

        // Gestion checkBox -> numeric
        private void UpdateNumericUpDownState(CheckBox checkBox, NumericUpDown numericUpDown) {
            if (checkBox.IsChecked == false) {
                numericUpDown.Value = 0;
            } else {
                numericUpDown.Value = 1;
            }
        }

        // Gestion numeric -> checkBox
        private void UpdateCheckBoxState(CheckBox checkBox, NumericUpDown numericUpDown) {
            if (numericUpDown.Value == 0) {
                checkBox.IsChecked = false;
            } else {
                checkBox.IsChecked = true;
            }
        }

        // Initialize comboBoxItem with the participants of the Operation's Tricount.
        private void InitializeCombobox() {
            // fetching users from the database
            List<User> users = GetUsersFromDatabase();
            foreach (var user in users) {
                // Create ComboBox
                ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = user.FullName };
                InitiatorComboBox.Items.Add(comboBoxItem);
            }

            // Rechercher l'élément correspondant dans la ComboBox
            ComboBoxItem defaultItem = InitiatorComboBox.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == vm.CurrentUser);

            // Si l'élément par défaut existe, le sélectionner
            if (defaultItem != null) {
                InitiatorComboBox.SelectedItem = defaultItem;
            }
        }

        // Return Users of the Operation's Tricount.
        private List<User> GetUsersFromDatabase() {
            Tricount tricount = vm.Tricount;
            List<User> participants = new List<User>();
            foreach (var user in tricount.GetParticipants()) {
                participants.Add(user);
                Console.WriteLine(user.FullName);
            }

            return participants;
        }
    }
}