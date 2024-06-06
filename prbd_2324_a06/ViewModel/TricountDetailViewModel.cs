using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace prbd_2324_a06.ViewModel;

public class TricountDetailViewModel : ViewModelCommon
{
    // Commandes
    public ICommand AddCommand { get; set; }
    public ICommand AddMySelf { get; set; }
    public ICommand AddEvery { get; set; }
    public ICommand AddTemplate { get; set; }
    public ICommand AddParticipant { get; set; }
    public ICommand SaveCommand { get; set; }
    public ICommand CancelCommand { get; set; }
    // Attributes et propriétés
    private ICollection<User> _participants;
    private string _title;
    public string Title {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _description;
    public string Description {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private DateTime _date;
    public DateTime Date {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    private Tricount _tricount;
    public Tricount Tricount {
        get => _tricount;
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }
    public TricountDetailViewModel(Tricount tricount, bool isNew) : base() {
        _tricount = tricount;
        _isNew = isNew;
        //SaveCommand = new RelayCommand(SaveAction, CanSaveAction);
        //CancelCommand = new RelayCommand(CancelAction, CanCancelAction);
    }

    //public override void SaveAction() {
    //    if (IsNew) {
    //        // Un petit raccourci ;-)
    //        Tricount.Title = Member.Pseudo;
    //        Context.Add(Tricount);
    //        IsNew = false;
    //    }
    //    Context.SaveChanges();
    //    RaisePropertyChanged();
    //    NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Member);
    //}

    //private bool CanSaveAction() {
    //    if (IsNew)
    //        return Member.Validate() && !HasErrors;
    //    return Member != null && Member.IsModified && !HasErrors;
    //}

    //public override void CancelAction() {
    //    ClearErrors();
    //    if (IsNew) {
    //        IsNew = false;
    //        NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Member);
    //    } else {
    //        Member.Reload();
    //        RaisePropertyChanged();
    //    }
    //}

    //private bool CanCancelAction() {
    //    return Member != null && (IsNew || Member.IsModified);
    //}



}
