using prbd_2324_a06.Model;
using PRBD_Framework;

namespace prbd_2324_a06.ViewModel;

public class TricountCardViewModel : ViewModelCommon
{

    private readonly Tricount _tricount;
    public Tricount Tricount {
        get => _tricount;
        init => SetProperty(ref _tricount, value);
    }
    public string Title => Tricount.Title;
    public string Description => Tricount.Description?? "No Description";
    public string CreatedBy => $"Created By {Tricount.GetCreatorName()} on {Tricount.CreatedAt.ToString("dd/MM/yyyy")}";
    public string WithFriends => Tricount.NumberOfParticipants() == 0 ? "With no friend" : $"With {Tricount.NumberOfParticipants()} friends";
    public string LastOperation => "Last operation on ";
    public string NumberOfOperations => "nb operations";
    public string TotalExpenses => $"Total expense : {Tricount.GetTotal()}";
    public string MyExpenses => $"My Expenses : {CurrentUser.GetMyExpenses(Tricount)}";
    public string MyBalance => $"My Balance : {CurrentUser.GetMyBalance(Tricount)}";

    public double Total => Tricount.GetTotal();
    public  TricountCardViewModel(Tricount tricount) {
        Tricount = tricount;
        Console.WriteLine(tricount.GetParticipants());
    }
}
