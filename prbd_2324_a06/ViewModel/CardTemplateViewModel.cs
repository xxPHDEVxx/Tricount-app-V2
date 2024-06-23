using Microsoft.IdentityModel.Tokens;
using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class CardTemplateViewModel : ViewModelCommon {
    public TricountDetailViewModel vm { get; set; }
    public Template Template { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand EditCommand { get; set; }
    private Visibility _visible;

    public Visibility Visible {
        get => _visible;
        set => SetProperty(ref _visible, value);
    }    
    
    public CardTemplateViewModel(TricountDetailViewModel tricountDetailViewModel, Template template) {
        vm = tricountDetailViewModel;
        Template = template;
        DeleteCommand = new RelayCommand(DeleteAction);
        EditCommand = new RelayCommand(EditAction);

    }

    private void DeleteAction() {

    }    
    private void EditAction() {

    }

  

}

