﻿using CalcBinding;
using Microsoft.IdentityModel.Tokens;
using NumericUpDownLib;
using prbd_2324_a06.Model;
using prbd_2324_a06.ViewModel;
using PRBD_Framework;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace prbd_2324_a06.View
{
    public partial class EditOperationView : WindowBase
    {
        public EditOperationView() {
            InitializeComponent();

            // initialisation dynamique des éléments graphiques
            InitializeCheckBox();
            InitializeCombobox();
            initializeTemplates();

            // fermeture de la fenêtre
            Register(App.Messages.MSG_CLOSE_WINDOW,
                Close);
        }

        // Initialise checkBox's template with the tricount's participants
        private void InitializeCheckBox() {
            // fetching users from the database
            List<User> users = vm.GetUsersTricount();

            foreach (var user in users) {
                // Create a new Grid for each user
                Grid userGrid = new Grid();
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                userGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Create CheckBox
                CheckBox checkBox = new CheckBox
                {
                    Content = user.FullName, Margin = new Thickness(2), Width = 80, IsChecked = true,
                    VerticalAlignment = VerticalAlignment.Center
                };
                vm.CheckBoxItems.Add(checkBox);
                Grid.SetColumn(checkBox, 0);
                userGrid.Children.Add(checkBox);

                // Create NumericUpDown
                NumericUpDown numericUpDown = new NumericUpDown {
                    Width = 60,
                    Value = 1,
                    MinValue = 0,
                    Margin = new Thickness(2),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                numericUpDown.ValueChanged += (sender, e) => UpdateCheckBoxState(checkBox, numericUpDown);
                Grid.SetColumn(numericUpDown, 1);
                userGrid.Children.Add(numericUpDown);

                // Create TextBlock
                TextBlock textBlock = new TextBlock
                    { Text = "0,00 €", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2) };
                Grid.SetColumn(textBlock, 2);
                userGrid.Children.Add(textBlock);

                // Add the userGrid to the ParticipantsPanel
                ParticipantsPanel.Children.Add(userGrid);

                // Gestionnaire d'événements pour la CheckBox
                checkBox.Checked += (sender, e) => UpdateNumericUpDownState(checkBox, numericUpDown);
                checkBox.Unchecked += (sender, e) => UpdateNumericUpDownState(checkBox, numericUpDown);
                checkBox.Checked += (sender, e) => vm.Validate();
                checkBox.Unchecked += (sender, e) => vm.Validate();
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
            List<User> users = vm.GetUsersTricount();
            foreach (var user in users) {
                // Create ComboBox
                ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = user.FullName };
                InitiatorComboBox.Items.Add(comboBoxItem);
            }

            // Trier les éléments de la ComboBox par ordre alphabétique
            List<ComboBoxItem> sortedItems = InitiatorComboBox.Items.Cast<ComboBoxItem>()
                .OrderBy(item => item.Content.ToString()).ToList();
            InitiatorComboBox.Items.Clear();
            foreach (var item in sortedItems) {
                InitiatorComboBox.Items.Add(item);
            }

            // Rechercher l'élément correspondant dans la ComboBox
            ComboBoxItem defaultItem = InitiatorComboBox.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == vm.Operation.Initiator.FullName);
            // Si l'élément par défaut existe, le sélectionner
            if (defaultItem != null) {
                InitiatorComboBox.SelectedItem = defaultItem;
            }
        }

        // Initialize comboBoxItem with the templates of the Operation's Tricount.
        private void initializeTemplates() {
            List<Template> templates = vm.GetTemplatesTricount();
            foreach (var template in templates) {
                // Create ComboBox
                ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = template.Title };
                TemplateComboBox.Items.Add(comboBoxItem);
            }

            // Trier les éléments de la ComboBox par ordre alphabétique
            List<ComboBoxItem> sortedItems = TemplateComboBox.Items.Cast<ComboBoxItem>()
                .OrderBy(item => item.Content.ToString()).ToList();
            TemplateComboBox.Items.Clear();
            foreach (var item in sortedItems) {
                TemplateComboBox.Items.Add(item);
            }

            // ajout Item par défaut
            ComboBoxItem defaultItem = new ComboBoxItem() { Content = "-- Choose a template --" };
            TemplateComboBox.Items.Add(defaultItem);
            TemplateComboBox.SelectedItem = defaultItem;
        }

        // Bouton Cancel
        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}