using PRBD_Framework;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;


namespace prbd_2324_a06.View
{
    /// <summary>
    /// Logique d'interaction pour TricountDetailView.xaml
    /// </summary>
    public partial class TricountDetailView : UserControlBase
    {
        private readonly TricountDetailViewModel _vm;
        private User GetCurrentUser() {
            return App.CurrentUser;
        }

        public TricountDetailView(Tricount tricount, bool isNew) {
            DataContext = _vm = new TricountDetailViewModel(tricount, isNew);
            InitializeComponent();
            InitializeComboBox();
        }

        private void InitializeComboBox() {
             PanelParticipants.Children.Clear();
            // fetching users from the database
            ObservableCollection<string> users = _vm.Participants;
            if (users != null && users.Count > 0) {
                
            foreach (var user in users) {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                    userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create Textbox
                TextBlock textBox = new TextBlock { Text = user, VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2), Width = 300,
                    TextAlignment = TextAlignment.Left
                };
                Grid.SetColumn(textBox, 0);
                userGrid.Children.Add(textBox);

                    if (user == GetCurrentUser().FullName) {
                        textBox.FontWeight = FontWeights.Bold;
                        TextBlock creator = new TextBlock {
                            Text = "(creator)", Margin = new Thickness(40,0,0,0), VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Left, FontWeight = FontWeights.Bold,
                        };
                        userGrid.Children.Add(creator);
                    }

                    // Create button
                    Button delete = new Button {
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(5)
                    };
                    //icon poubelle avec fontawesome
                    var deleteIcon = new FontAwesome.WPF.FontAwesome {
                        Icon = FontAwesome.WPF.FontAwesomeIcon.Trash,
                        Foreground = Brushes.Red,
                        Width = 16,
                        Height = 16,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    delete.Content = deleteIcon;
                    delete.Click += (s, e) => {
                        //delete un participant
                        //de la liste
                        _vm.Participants.Remove(user);
                        //du panel
                        PanelParticipants.Children.Remove(userGrid);
                        // et on le remet dans la liste
                        _vm.Users.Add(user);
                    };
                    if (user != GetCurrentUser().FullName) {
                        Grid.SetColumn(delete, 1);
                        userGrid.Children.Add(delete);
                        
                    }


                    // Add the userGrid to the ParticipantsPanel
                    PanelParticipants.Children.Add(userGrid);
            }
            }

        }

        private void btnAddMe_Click(object sender, RoutedEventArgs e) {
                 _vm.AddMySelf.Execute(null);
                InitializeComboBox();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            _vm.AddCommand.Execute(null);
            InitializeComboBox();
        }

        private void btnAddAll_Click(object sender, RoutedEventArgs e) {
            _vm.AddEvery.Execute(null);
            _vm.Users.Clear();
            InitializeComboBox();

        }
    }
}
