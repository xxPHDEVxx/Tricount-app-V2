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
    public string Description => Tricount.Description;


    public  TricountCardViewModel(Tricount tricount) {
        Tricount = tricount;
    }
}
