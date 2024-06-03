using NumericUpDownLib;
using PRBD_Framework;
using System.Windows;
using System.Windows.Controls;

namespace prbd_2324_a06.View
{
    public partial class AddOperationView : WindowBase {
        public AddOperationView() {
            InitializeComponent();
            LoadUsers();
        }
        
        // Initialise check box template avec participants
        private void LoadUsers()
        {
            // Simulate fetching users from the database
            List<string> users = GetUsersFromDatabase();

            int row = 0;
            foreach (var user in users)
            {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create CheckBox
                CheckBox checkBox = new CheckBox { Content = user, Margin = new Thickness(5), IsChecked = true};
                Grid.SetColumn(checkBox, 0);
                userGrid.Children.Add(checkBox);

                // Create NumericUpDown
                NumericUpDown numericUpDown = new NumericUpDown
                {
                    Width = 60,
                    Value = 1,
                    MinValue = 0,
                    Margin = new Thickness(5)
                };
                numericUpDown.ValueChanged += (sender, e) => 
                {
                    if (numericUpDown.Value == 0)
                    {
                        checkBox.IsChecked = false;
                    }
                    else
                    {
                        checkBox.IsChecked = true;
                    }
                };
                Grid.SetColumn(numericUpDown, 1);
                userGrid.Children.Add(numericUpDown);

                // Create TextBlock
                TextBlock textBlock = new TextBlock { Text = "0,00 €", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5) };
                Grid.SetColumn(textBlock, 2);
                userGrid.Children.Add(textBlock);
                

                // Add the userGrid to the ParticipantsPanel
                ParticipantsPanel.Children.Add(userGrid);

                row++;
            }
        }

        // Return Users of the Operation's Tricount
        private List<string> GetUsersFromDatabase()
        {
            // This method should actually fetch users from your database
            return new List<string> { "Benoît", "Boris", "Marc", "Xavier" };
        }
    }
}