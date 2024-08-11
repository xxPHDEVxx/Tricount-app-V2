using NumericUpDownLib;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_a06.View
{
    public partial class OperationView
    {
        private readonly OperationViewModel _vm;

        public OperationView(Operation operation) {
            InitializeComponent();
            DataContext = _vm = new OperationViewModel(operation);

            // fermeture de la fenêtre
            Register<Operation>(App.Messages.MSG_CLOSE_OPERATION_WINDOW, _ => {
                Close();
            });
        }

        // Initialise checkBox's template with the tricount's participants
        /*private void InitializeCheckBox() {
            // fetching from the database
            List<User> users = _vm.GetUsersTricount();
            List<Repartition> repartitions = _vm.GetRepartitions();

            foreach (User user in users) {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create CheckBox
                CheckBox checkBox = new CheckBox {
                    Content = user.FullName, Margin = new Thickness(2), Width = 80,
                    VerticalAlignment = VerticalAlignment.Center
                };
                _vm.CheckBoxItems.Add(checkBox);
                Grid.SetColumn(checkBox, 0);
                userGrid.Children.Add(checkBox);

                // Create NumericUpDown
                NumericUpDown numericUpDown = new NumericUpDown {
                    Width = 40,
                    Value = repartitions.Find(r => r.UserId == user.UserId) != null
                        ? repartitions.Find(r => r.UserId == user.UserId).Weight
                        : 0,
                    MinValue = 0,
                    Margin = new Thickness(2),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Name = user.FullName,
                };
                _vm.Numerics.Add(numericUpDown);
                Grid.SetColumn(numericUpDown, 1);
                userGrid.Children.Add(numericUpDown);

                // Create TextBlock
                TextBlock textBlock = new TextBlock {
                    Text = "0,00 €", VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2), Width = 50, FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Left
                };
                _vm.TextBlocks.Add(textBlock);
                Grid.SetColumn(textBlock, 2);
                userGrid.Children.Add(textBlock);

                // Add the userGrid to the ParticipantsPanel
                ParticipantsPanel.Children.Add(userGrid);


                // Gestionnaire d'événements pour la CheckBox
                checkBox.Checked += (sender, e) => {
                    UpdateNumericUpDownState(checkBox, numericUpDown);
                    _vm.Validate();
                    _vm.CalculAmount();
                };
                checkBox.Unchecked += (sender, e) => {
                    UpdateNumericUpDownState(checkBox, numericUpDown);
                    _vm.Validate();
                    _vm.CalculAmount();
                };

                // Gestionnaire d'évènements pour le Numeric
                numericUpDown.ValueChanged += (sender, e) => {
                    UpdateCheckBoxState(checkBox, numericUpDown);
                    _vm.Validate();
                    _vm.CalculAmount();
                };

                UpdateCheckBoxState(checkBox, numericUpDown);
            }
            _vm.CalculAmount();
        }

        // Gestion checkBox -> numeric
        private void UpdateNumericUpDownState(CheckBox checkBox, NumericUpDown numericUpDown) {
            numericUpDown.Value = checkBox.IsChecked == false ? 0 : 1;
        }

        // Gestion numeric -> checkBox
        private void UpdateCheckBoxState(CheckBox checkBox, NumericUpDown numericUpDown) {
            checkBox.IsChecked = numericUpDown.Value != 0;
        }*/

        // Bouton Cancel
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        
    }
}