﻿using Microsoft.Extensions.Options;

namespace prbd_2324_a06.ViewModel
{
    public class AddOperationViewModel : ViewModelCommon
    {
        public AddOperationViewModel() : base() {
        }
        // Attributes
        private string _title;
        private string _amount;

        // Property
        public string Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => Validate());
        }
        public string Title {
            get => _title;
            set => SetProperty(ref _title, value, () => Validate());
        }

        // Méthode de validation
        public override bool Validate() {
            ClearErrors();
            
            if (string.IsNullOrEmpty(Title))
                AddError(nameof(Title), "required");
            else {
                if (IsValidAmount(Amount) != "" ) {
                    AddError(nameof(Amount), "Must contain only numbers");
                }
                else if (double.Parse(Amount) < 0.01) {
                    AddError(nameof(Amount), "minimum 1 cent");
                }
            }
            return !HasErrors;
        }
        
        private string IsValidAmount(string amount) {
            // Regex to match a string that contains only digits
            var onlyNumbers = new System.Text.RegularExpressions.Regex(@"^\d+(\.\d+)?$");

    
            // Check if the password matches the regex
            if (!onlyNumbers.IsMatch(amount))
                return "Must contain only numbers";
    
            return "";
        }
    }
}