using prbd_2324_a06.Model;
using PRBD_Framework;
using System.Windows.Media;

namespace prbd_2324_a06.ViewModel;

public class TricountCardViewModel : ViewModelCommon
{

    private readonly Tricount _tricount;
    private Brush _backgroundColor;

    public Brush BackgroundColor {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }
    public Tricount Tricount {
        get => _tricount;
        init => SetProperty(ref _tricount, value);
    }
    public string Title => Tricount.Title;
    public string Description => Tricount.Description?? "No Description";
    public string CreatedBy => $"Created By {Tricount.GetCreatorName()} on {Tricount.CreatedAt.ToString("dd/MM/yyyy")}";
    public string WithFriends => Tricount.NumberOfParticipants() == 0 ? "With no friend" : $"With {Tricount.NumberOfParticipants()} friends";
    public string LastOperation => Tricount.GetOperations().Count() == 0 ? "" : $"Last operation on {Tricount.GetLatestOperation().OperationDate.ToString("dd/MM/yyyy")}";
    public string NumberOfOperations => Tricount.GetOperations().Count() == 0 ? "No operation" : $" {Tricount.GetOperations().Count()} operations";
    public string TotalExpenses => $"Total expense : {Tricount.GetTotal()}";
    public string MyExpenses => $"My Expenses : {CurrentUser.GetMyExpenses(Tricount)}";
    public string MyBalance => $"My Balance : {CurrentUser.GetMyBalance(Tricount)}";

    public double Total => Tricount.GetTotal();
    public  TricountCardViewModel(Tricount tricount) {
        Tricount = tricount;
        UpdateBackground();
    }

    private void UpdateBackground() {
        if (CurrentUser.GetMyBalance(Tricount) == 0) {
            BackgroundColor = Brushes.LightGray;
        } else if(CurrentUser.GetMyBalance(Tricount) < 0) {
            BackgroundColor = Brushes.LightSalmon;
        } 
        else {
            BackgroundColor= Brushes.LightGreen;
        }
    }
}
