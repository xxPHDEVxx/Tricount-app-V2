﻿using Microsoft.IdentityModel.Tokens;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class CardParticipantViewModel : ViewModelCommon {
    public TricountDetailViewModel vm { get; set; }
    public User User { get; set; }
    public ICommand DeleteCommand { get; set; }
    private Visibility _visible;

    public Visibility Visible {
        get => _visible;
        set => SetProperty(ref _visible, value);
    }    
    
    public CardParticipantViewModel(TricountDetailViewModel tricountDetailViewModel, User user) {
        vm = tricountDetailViewModel;
        User = user;
        DeleteCommand = new RelayCommand(DeleteAction);
        Visible = User == App.CurrentUser || User.GetOperations(vm.Tricount).Count() > 0 ? Visibility.Hidden : Visibility.Visible;
    }

    private void DeleteAction() {
        vm.Participants.Remove(this);
        vm.Users.Add(User);
    }

    public string Current => App.CurrentUser == User ? "(creator)" : "";
    public string NbExpenses => App.CurrentUser != User && User.GetOperations(vm.Tricount).Count() > 0 ? $"({User.GetOperations(vm.Tricount).Count().ToString()} expenses)" : "";  
  

    public bool IsEquals(CardParticipantViewModel other) {
        return User == other.User;
    }
}

